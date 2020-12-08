
namespace steam_shortcut_manager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.steamDirTxt = new System.Windows.Forms.TextBox();
            this.steamDirFind = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.steamLibList = new System.Windows.Forms.CheckedListBox();
            this.addSteamLibBtn = new System.Windows.Forms.Button();
            this.gameListSelectAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.gameListSelectNone = new System.Windows.Forms.Button();
            this.gameListInvertSelect = new System.Windows.Forms.Button();
            this.createShortcutsBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gameList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam Installation Directory";
            // 
            // steamDirTxt
            // 
            this.steamDirTxt.Location = new System.Drawing.Point(15, 25);
            this.steamDirTxt.Name = "steamDirTxt";
            this.steamDirTxt.Size = new System.Drawing.Size(266, 20);
            this.steamDirTxt.TabIndex = 1;
            // 
            // steamDirFind
            // 
            this.steamDirFind.Location = new System.Drawing.Point(287, 25);
            this.steamDirFind.Name = "steamDirFind";
            this.steamDirFind.Size = new System.Drawing.Size(30, 20);
            this.steamDirFind.TabIndex = 2;
            this.steamDirFind.Text = "...";
            this.steamDirFind.UseVisualStyleBackColor = true;
            this.steamDirFind.Click += new System.EventHandler(this.steamDirFind_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Search Directories (directory containing steamapps folder)";
            // 
            // steamLibList
            // 
            this.steamLibList.FormattingEnabled = true;
            this.steamLibList.Location = new System.Drawing.Point(15, 77);
            this.steamLibList.Name = "steamLibList";
            this.steamLibList.Size = new System.Drawing.Size(302, 139);
            this.steamLibList.TabIndex = 4;
            // 
            // addSteamLibBtn
            // 
            this.addSteamLibBtn.Location = new System.Drawing.Point(15, 222);
            this.addSteamLibBtn.Name = "addSteamLibBtn";
            this.addSteamLibBtn.Size = new System.Drawing.Size(101, 23);
            this.addSteamLibBtn.TabIndex = 5;
            this.addSteamLibBtn.Text = "Add Directory";
            this.addSteamLibBtn.UseVisualStyleBackColor = true;
            this.addSteamLibBtn.Click += new System.EventHandler(this.addSteamLibBtn_Click);
            // 
            // gameListSelectAll
            // 
            this.gameListSelectAll.Location = new System.Drawing.Point(15, 405);
            this.gameListSelectAll.Name = "gameListSelectAll";
            this.gameListSelectAll.Size = new System.Drawing.Size(94, 23);
            this.gameListSelectAll.TabIndex = 6;
            this.gameListSelectAll.Text = "Select All";
            this.gameListSelectAll.UseVisualStyleBackColor = true;
            this.gameListSelectAll.Click += new System.EventHandler(this.gameListSelectAll_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Discovered Games";
            // 
            // gameListSelectNone
            // 
            this.gameListSelectNone.Location = new System.Drawing.Point(115, 405);
            this.gameListSelectNone.Name = "gameListSelectNone";
            this.gameListSelectNone.Size = new System.Drawing.Size(92, 23);
            this.gameListSelectNone.TabIndex = 9;
            this.gameListSelectNone.Text = "Select None";
            this.gameListSelectNone.UseVisualStyleBackColor = true;
            this.gameListSelectNone.Click += new System.EventHandler(this.gameListSelectNone_Click);
            // 
            // gameListInvertSelect
            // 
            this.gameListInvertSelect.Location = new System.Drawing.Point(213, 405);
            this.gameListInvertSelect.Name = "gameListInvertSelect";
            this.gameListInvertSelect.Size = new System.Drawing.Size(104, 23);
            this.gameListInvertSelect.TabIndex = 10;
            this.gameListInvertSelect.Text = "Invert Selection";
            this.gameListInvertSelect.UseVisualStyleBackColor = true;
            this.gameListInvertSelect.Click += new System.EventHandler(this.gameListInvertSelect_Click);
            // 
            // createShortcutsBtn
            // 
            this.createShortcutsBtn.Location = new System.Drawing.Point(12, 434);
            this.createShortcutsBtn.Name = "createShortcutsBtn";
            this.createShortcutsBtn.Size = new System.Drawing.Size(305, 23);
            this.createShortcutsBtn.TabIndex = 11;
            this.createShortcutsBtn.Text = "Create Start Menu Shortcuts";
            this.createShortcutsBtn.UseVisualStyleBackColor = true;
            this.createShortcutsBtn.Click += new System.EventHandler(this.createShortcutsBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(216, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Search Directories";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gameList
            // 
            this.gameList.CheckBoxes = true;
            this.gameList.Location = new System.Drawing.Point(15, 275);
            this.gameList.Name = "gameList";
            this.gameList.Size = new System.Drawing.Size(302, 124);
            this.gameList.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 469);
            this.Controls.Add(this.gameList);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.createShortcutsBtn);
            this.Controls.Add(this.gameListInvertSelect);
            this.Controls.Add(this.gameListSelectNone);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gameListSelectAll);
            this.Controls.Add(this.addSteamLibBtn);
            this.Controls.Add(this.steamLibList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.steamDirFind);
            this.Controls.Add(this.steamDirTxt);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Steam Shortcut Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox steamDirTxt;
        private System.Windows.Forms.Button steamDirFind;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox steamLibList;
        private System.Windows.Forms.Button addSteamLibBtn;
        private System.Windows.Forms.Button gameListSelectAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button gameListSelectNone;
        private System.Windows.Forms.Button gameListInvertSelect;
        private System.Windows.Forms.Button createShortcutsBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView gameList;
    }
}

