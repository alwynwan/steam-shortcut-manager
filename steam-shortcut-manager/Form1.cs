using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace steam_shortcut_manager
{
    public partial class Form1 : Form
    {
        XDocument AppInfo;
        public Form1()
        {
            InitializeComponent();
        }

        private static class CacheToXml
        {
            public static XDocument Dump(string inputFile)
            {
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (inputStream.Length < sizeof(int))
                    {
                        throw new ApplicationException("Cache file is too small");
                    }

                    if (inputStream.Length >= (50 * 1024 * 1024))
                    {
                        throw new ApplicationException("Cache file is too large");
                    }

                    BinaryReader reader = new BinaryReader(inputStream);

                    uint magic = reader.ReadUInt32();
                    if (magic != 0x07564427)
                    {
                        throw new ApplicationException("Cache file is of unsupported format");
                    }

                    MemoryStream ms = new MemoryStream();

                    XmlWriterSettings writerSettings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };
                    using (XmlWriter writer = XmlWriter.Create(ms, writerSettings))
                    {
                        DumpAppDataCacheV3(reader, writer);
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    return XDocument.Load(ms);
                }
            }

            private static void DumpAppDataCacheV3(BinaryReader reader, XmlWriter writer)
            {
                int universe = reader.ReadInt32();

                writer.WriteStartElement("AppDataCache");
                writer.WriteAttributeString("universe", universe.ToString(NumberFormatInfo.InvariantInfo));

                for (int appId = reader.ReadInt32(); appId != 0; appId = reader.ReadInt32())
                {
                    int dataSize = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(dataSize);
                    if (data.Length != dataSize)
                        throw new EndOfStreamException();

                    writer.WriteStartElement("Application");
                    writer.WriteAttributeString("id", appId.ToString(NumberFormatInfo.InvariantInfo));
                    writer.WriteAttributeString("dataSize", dataSize.ToString(NumberFormatInfo.InvariantInfo));

                    using (BinaryReader dataReader = new BinaryReader(new MemoryStream(data, false)))
                    {
                        int state = dataReader.ReadInt32();
                        DateTime lastChange = new DateTime((dataReader.ReadUInt32() + 62135596800) * TimeSpan.TicksPerSecond,
                            DateTimeKind.Utc);
                        long accessToken = dataReader.ReadInt64();
                        byte[] sha1 = dataReader.ReadBytes(20);
                        int changeNumber = dataReader.ReadInt32();

                        writer.WriteAttributeString("state", GetAppInfoState(state).ToString(NumberFormatInfo.InvariantInfo));
                        writer.WriteAttributeString("lastChange", lastChange.ToString("o", DateTimeFormatInfo.InvariantInfo));
                        writer.WriteAttributeString("accessToken", accessToken.ToString(NumberFormatInfo.InvariantInfo));
                        writer.WriteAttributeString("sha1", BitConverter.ToString(sha1).Replace("-", ""));
                        writer.WriteAttributeString("changeNumber", changeNumber.ToString(NumberFormatInfo.InvariantInfo));

                        DumpAppDataCacheSections(dataReader, writer);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            private static void DumpAppDataCacheSections(BinaryReader reader, XmlWriter writer)
            {
                writer.WriteStartElement("Section");
                writer.WriteAttributeString("type", "section");

                DumpKeyValues(reader, writer);

                writer.WriteEndElement();
            }


            private static string GetAppInfoState(int value)
            {
                switch (value)
                {
                    case 1:
                        return "Unavailable";
                    case 2:
                        return "Available";
                    default:
                        Trace.Fail("Unknown application info state: " + value.ToString(NumberFormatInfo.InvariantInfo));
                        return value.ToString(NumberFormatInfo.InvariantInfo);
                }
            }

            private static void DumpKeyValues(BinaryReader reader, XmlWriter writer)
            {
                for (byte valueType = reader.ReadByte(); valueType != 8; valueType = reader.ReadByte())
                {
                    string name = ReadString(reader);
                    if (valueType == 0)
                    {
                        writer.WriteStartElement("Key");
                        if (!string.IsNullOrEmpty(name))
                        {
                            writer.WriteAttributeString("name", name);
                        }

                        DumpKeyValues(reader, writer);

                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement("Value");
                        writer.WriteAttributeString("name", name);
                        switch (valueType)
                        {
                            case 1:
                                string valueString = ReadString(reader);
                                writer.WriteAttributeString("type", "string");
                                writer.WriteString(valueString);
                                break;

                            case 2:
                                int valueInt32 = reader.ReadInt32();
                                writer.WriteAttributeString("type", "int32");
                                writer.WriteString(valueInt32.ToString(NumberFormatInfo.InvariantInfo));
                                break;

                            case 3:
                                float valueSingle = reader.ReadSingle();
                                writer.WriteAttributeString("type", "single");
                                writer.WriteString(valueSingle.ToString(NumberFormatInfo.InvariantInfo));
                                break;

                            case 4:
                                throw new NotSupportedException("Pointers cannot be encoded in the binary format.");

                            case 5:
                                string valueWString = ReadWideString(reader);
                                writer.WriteAttributeString("type", "wstring");
                                writer.WriteString(valueWString);
                                break;

                            case 6:
                                byte valueColorR = reader.ReadByte();
                                byte valueColorG = reader.ReadByte();
                                byte valueColorB = reader.ReadByte();
                                writer.WriteAttributeString("type", "color");
                                writer.WriteString(valueColorR.ToString(NumberFormatInfo.InvariantInfo) + " " +
                                                   valueColorG.ToString(NumberFormatInfo.InvariantInfo) + " " +
                                                   valueColorB.ToString(NumberFormatInfo.InvariantInfo));
                                break;

                            case 7:
                                ulong valueUInt64 = reader.ReadUInt64();
                                writer.WriteAttributeString("type", "uint64");
                                writer.WriteString(valueUInt64.ToString(NumberFormatInfo.InvariantInfo));
                                break;

                            default:
                                throw new NotImplementedException("The value type " + valueType +
                                                                  " has not been implemented.");
                        }
                        writer.WriteEndElement();
                    }
                }
            }

            private static string ReadString(BinaryReader reader)
            {
                byte[] buffer;
                int bufferLength;

                using (MemoryStream ms = new MemoryStream())
                {
                    byte b;
                    while ((b = reader.ReadByte()) != 0)
                        ms.WriteByte(b);

                    buffer = ms.GetBuffer();
                    bufferLength = (int)ms.Length;
                }

                string s = Encoding.UTF8.GetString(buffer, 0, bufferLength);

                s = s.Replace("\v", "\\v");

                return s;
            }

            private static string ReadWideString(BinaryReader reader)
            {
                StringBuilder sb = new StringBuilder();
                for (char value = (char)reader.ReadUInt16(); value != 0; value = (char)reader.ReadUInt16())
                {
                    if (value == '\v')
                        sb.Append("\\v");
                    else
                        sb.Append(value);
                }
                return sb.ToString();
            }
        }

        private struct GameInfo
        {
            public GameInfo(string name, string dir)
            {
                gameName = name;
                installDir = dir;
            }
            public string gameName { get; set; }
            public string installDir { get; set; }
        }

        private string CleanString(string input)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                input = input.Replace(c.ToString(), "");
            }

            return input;
        }

        private string FindSteamInstall()
        {
            return Properties.Settings.Default.steamDir.Equals(string.Empty) ?
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", "").ToString() + "\\" :
                Properties.Settings.Default.steamDir;
        }

        private string[] FindSteamLibraries()
        {
            List<string> steamLibs = new List<string>();

            steamLibs.Add(Properties.Settings.Default.steamDir);

            if (System.IO.File.Exists(steamDirTxt.Text + "\\steamapps\\libraryfolders.vdf"))
            {
                var contents = System.IO.File.ReadAllText(steamDirTxt.Text + "\\steamapps\\libraryfolders.vdf");
                Regex foldersPattern = new Regex("\"\\d\"\\t\\t\"(.*)\"");
                Match foldersMatch = foldersPattern.Match(contents);

                while (foldersMatch.Success)
                {
                    var libDir = foldersMatch.Groups[1].Value.Replace("\\\\", "\\") + "\\";
                    steamLibs.Add(libDir);
                    foldersMatch = foldersMatch.NextMatch();
                }
            }

            return steamLibs.ToArray();
        }

        private void AddSteamLibrary()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = steamDirTxt.Text;
                fbd.Description = "Locate a valid Steam Library (directory which contains a steamapps folder)";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    steamLibList.Items.Add(fbd.SelectedPath);
                }
            }
        }

        private Dictionary<int, GameInfo> FindInstalledGames()
        {
            Dictionary<int, GameInfo> installedGames = new Dictionary<int, GameInfo>();

            for (var i = 0; i < steamLibList.CheckedItems.Count; i++)
            {
                var libDir = steamLibList.CheckedItems[i].ToString();

                if (libDir[libDir.Length - 1] != '\\')
                    libDir += '\\';

                var manifests = Directory.GetFiles(steamLibList.CheckedItems[i].ToString() + "steamapps\\", "appmanifest_*.acf");

                foreach (var manifest in manifests)
                {
                    var manifestContents = System.IO.File.ReadAllText(manifest);

                    Regex manifestRegex = new Regex("\"appid\"\\t\\t\"(\\d+)\".*\"name\"\\t\\t\"(.*)?\"\\n\\t\"StateFlags\".*\"installdir\"\\t\\t\"(.*)\"\\n\\t\"LastUpdated\".*\"BytesToDownload\"\\t\\t\"(\\d+)\"\\n\\t\"BytesDownloaded\"\\t\\t\"(\\d+)\"", RegexOptions.Singleline);
                    var manifestMatch = manifestRegex.Match(manifestContents);

                    if (manifestMatch.Success)
                    {
                        var appId = int.Parse(manifestMatch.Groups[1].Value);
                        var appName = manifestMatch.Groups[2].Value;
                        var installDir = steamLibList.CheckedItems[i].ToString() + $"steamapps\\common\\{manifestMatch.Groups[3].Value}";
                        var bytesToDownload = long.Parse(manifestMatch.Groups[4].Value);
                        var bytesDownloaded = long.Parse(manifestMatch.Groups[5].Value);

                        // Game is fully installed
                        if (bytesDownloaded >= bytesToDownload)
                        {
                            installedGames.Add(appId, new GameInfo(appName, installDir));
                        }
                    }
                }
            }

            return installedGames;
        }

        private XDocument GetAppInfo()
        {
            if (AppInfo == null)
            {
                AppInfo = CacheToXml.Dump(Path.Combine(Properties.Settings.Default.steamDir, @"appcache\appinfo.vdf"));
            }
            return AppInfo;
        }

        private void CreateShortcut(int id, GameInfo gameInfo)
        {
            string startMenuDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Steam";

            XDocument data = GetAppInfo();

            var appcache = data.Descendants("Application").SingleOrDefault(app => int.Parse(app.Attribute("id").Value) == id);
            var appinfo = appcache.Descendants("Key").Where(app => app.Attribute("name").Value == "appinfo");
            var appinfoCommon = appinfo.Descendants("Key").Where(app => app.Attribute("name").Value == "common");
            var clienticon = appinfoCommon.Descendants("Value").Where(app => app.Attribute("name").Value == "clienticon").ToArray();

            var configs = appcache.Descendants("Key").Where(app => app.Attribute("name").Value == "config");
            var launch = configs.Descendants("Key").Where(key => key.Attribute("name").Value == "launch").ToArray();

            var launchConfigs = launch[0].ToString();

            Regex dataRegex = new Regex("name=\\\"executable\\\" type=\\\"string\\\">(.*?)<\\/Value>\\r\\n");
            var dataMatch = dataRegex.Match(launchConfigs);

            if (dataMatch.Success)
            {
                var shortcutLocation = startMenuDir + "\\" + CleanString(gameInfo.gameName) + ".url";

                using (StreamWriter writer = new StreamWriter(shortcutLocation))
                {
                    writer.WriteLine("[{000214A0-0000-0000-C000-000000000046}]");
                    writer.WriteLine("Prop3=19,0");
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("IDList=");
                    writer.WriteLine($"IconIndex=0");
                    writer.WriteLine($"URL=steam://rungameid/{id}");
                    var iconPath = Properties.Settings.Default.steamDir + "steam\\games\\" + clienticon[0].Value + ".ico";
                    writer.WriteLine($"IconFile={iconPath}");
                    writer.Flush();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            steamDirTxt.Text = FindSteamInstall();
            Properties.Settings.Default.steamDir = steamDirTxt.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();

            if (steamDirTxt.Text.Equals(string.Empty))
                return;

            var steamLibs = FindSteamLibraries();

            foreach (var lib in steamLibs)
            {
                steamLibList.Items.Add(lib, true);
            }
        }

        private void steamDirFind_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Steam Executable|Steam.exe";
                ofd.RestoreDirectory = true;
                ofd.Title = "Locate Steam Executable";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var dirEnd = ofd.FileName.LastIndexOf('\\');
                    steamDirTxt.Text = ofd.FileName.Substring(0, dirEnd + 1);
                    Properties.Settings.Default.steamDir = steamDirTxt.Text;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                }
            }
        }

        private void addSteamLibBtn_Click(object sender, EventArgs e)
        {
            AddSteamLibrary();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var installedGames = FindInstalledGames();
            var installedGamesSorted = installedGames.OrderBy(pair => pair.Value.gameName).Take(installedGames.Count);

            foreach (var game in installedGamesSorted)
            {
                gameList.Nodes.Add(game.Key.ToString(), game.Value.gameName);
            }
        }

        private void gameListSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gameList.Nodes.Count; i++)
            {
                gameList.Nodes[i].Checked = true;
            }
        }

        private void gameListSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gameList.Nodes.Count; i++)
            {
                gameList.Nodes[i].Checked = false;
            }
        }

        private void gameListInvertSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gameList.Nodes.Count; i++)
            {
                gameList.Nodes[i].Checked = !gameList.Nodes[i].Checked;
            }
        }

        private void createShortcutsBtn_Click(object sender, EventArgs e)
        {

            foreach (var game in FindInstalledGames())
            {
                try
                {
                    var gameNode = gameList.Nodes.Find(game.Key.ToString(), false);
                    if (gameNode.Length > 0 && gameNode[0].Checked)
                    {
                        CreateShortcut(game.Key, game.Value);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
