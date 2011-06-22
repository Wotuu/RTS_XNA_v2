namespace GameServer
{
    partial class ServerUI
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
            this.ServerMenuStrip = new System.Windows.Forms.MenuStrip();
            this.ServerMenuTab = new System.Windows.Forms.ToolStripMenuItem();
            this.StartServerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartGameServer = new System.Windows.Forms.ToolStripMenuItem();
            this.StartChatServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.StopServerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopGameServer = new System.Windows.Forms.ToolStripMenuItem();
            this.StopChatServer = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClientsListView = new System.Windows.Forms.ListView();
            this.Username = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClientsLbl = new System.Windows.Forms.Label();
            this.ClientsPanel = new System.Windows.Forms.Panel();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.SentMessagesListView = new System.Windows.Forms.ListView();
            this.SentTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SentMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReceivedMessagesListView = new System.Windows.Forms.ListView();
            this.ReceivedTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReceivedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReceivedMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReceivedMessagesLbl = new System.Windows.Forms.Label();
            this.ChatServerStatusLbl = new System.Windows.Forms.Label();
            this.ChatServerLbl = new System.Windows.Forms.Label();
            this.GameServerLabel = new System.Windows.Forms.Label();
            this.GameServerStatusLbl = new System.Windows.Forms.Label();
            this.ViewGameServerClientsBtn = new System.Windows.Forms.Button();
            this.ViewChatServerClientsBtn = new System.Windows.Forms.Button();
            this.SentMessagesLbl = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ServerMenuStrip.SuspendLayout();
            this.ClientsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerMenuStrip
            // 
            this.ServerMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerMenuTab});
            this.ServerMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.ServerMenuStrip.Name = "ServerMenuStrip";
            this.ServerMenuStrip.Size = new System.Drawing.Size(1003, 24);
            this.ServerMenuStrip.TabIndex = 0;
            this.ServerMenuStrip.Text = "menuStrip1";
            // 
            // ServerMenuTab
            // 
            this.ServerMenuTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartServerMenuItem,
            this.toolStripSeparator2,
            this.StopServerMenuItem,
            this.ExitMenuItem});
            this.ServerMenuTab.Name = "ServerMenuTab";
            this.ServerMenuTab.Size = new System.Drawing.Size(51, 20);
            this.ServerMenuTab.Text = "Server";
            this.ServerMenuTab.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // StartServerMenuItem
            // 
            this.StartServerMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartGameServer,
            this.StartChatServer});
            this.StartServerMenuItem.Name = "StartServerMenuItem";
            this.StartServerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.StartServerMenuItem.Text = "Start Server";
            // 
            // StartGameServer
            // 
            this.StartGameServer.Name = "StartGameServer";
            this.StartGameServer.Size = new System.Drawing.Size(140, 22);
            this.StartGameServer.Text = "Game Server";
            this.StartGameServer.Click += new System.EventHandler(this.StartGameServer_Click);
            // 
            // StartChatServer
            // 
            this.StartChatServer.Name = "StartChatServer";
            this.StartChatServer.Size = new System.Drawing.Size(140, 22);
            this.StartChatServer.Text = "Chat Server";
            this.StartChatServer.Click += new System.EventHandler(this.StartChatServer_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(130, 6);
            // 
            // StopServerMenuItem
            // 
            this.StopServerMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StopGameServer,
            this.StopChatServer});
            this.StopServerMenuItem.Name = "StopServerMenuItem";
            this.StopServerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.StopServerMenuItem.Text = "Stop Server";
            // 
            // StopGameServer
            // 
            this.StopGameServer.Enabled = false;
            this.StopGameServer.Name = "StopGameServer";
            this.StopGameServer.Size = new System.Drawing.Size(140, 22);
            this.StopGameServer.Text = "Game Server";
            this.StopGameServer.Click += new System.EventHandler(this.StopGameServer_Click);
            // 
            // StopChatServer
            // 
            this.StopChatServer.Enabled = false;
            this.StopChatServer.Name = "StopChatServer";
            this.StopChatServer.Size = new System.Drawing.Size(140, 22);
            this.StopChatServer.Text = "Chat Server";
            this.StopChatServer.Click += new System.EventHandler(this.StopChatServer_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(133, 22);
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ClientsListView
            // 
            this.ClientsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Username,
            this.IP});
            this.ClientsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClientsListView.Location = new System.Drawing.Point(0, 0);
            this.ClientsListView.MultiSelect = false;
            this.ClientsListView.Name = "ClientsListView";
            this.ClientsListView.Size = new System.Drawing.Size(304, 274);
            this.ClientsListView.TabIndex = 1;
            this.ClientsListView.UseCompatibleStateImageBehavior = false;
            this.ClientsListView.View = System.Windows.Forms.View.Details;
            // 
            // Username
            // 
            this.Username.Text = "Username";
            this.Username.Width = 117;
            // 
            // IP
            // 
            this.IP.Text = "IP";
            this.IP.Width = 117;
            // 
            // ClientsLbl
            // 
            this.ClientsLbl.AutoSize = true;
            this.ClientsLbl.Location = new System.Drawing.Point(3, 2);
            this.ClientsLbl.Name = "ClientsLbl";
            this.ClientsLbl.Size = new System.Drawing.Size(38, 13);
            this.ClientsLbl.TabIndex = 2;
            this.ClientsLbl.Text = "Clients";
            // 
            // ClientsPanel
            // 
            this.ClientsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ClientsPanel.Controls.Add(this.splitContainer5);
            this.ClientsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ClientsPanel.Location = new System.Drawing.Point(693, 0);
            this.ClientsPanel.Name = "ClientsPanel";
            this.ClientsPanel.Size = new System.Drawing.Size(306, 307);
            this.ClientsPanel.TabIndex = 3;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.IsSplitterFixed = true;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.ClientsLbl);
            this.splitContainer5.Panel1MinSize = 15;
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.ClientsListView);
            this.splitContainer5.Size = new System.Drawing.Size(304, 305);
            this.splitContainer5.SplitterDistance = 27;
            this.splitContainer5.TabIndex = 12;
            // 
            // SentMessagesListView
            // 
            this.SentMessagesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SentTime,
            this.SentHeader,
            this.SentMessage});
            this.SentMessagesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SentMessagesListView.Location = new System.Drawing.Point(0, 0);
            this.SentMessagesListView.MultiSelect = false;
            this.SentMessagesListView.Name = "SentMessagesListView";
            this.SentMessagesListView.Size = new System.Drawing.Size(503, 300);
            this.SentMessagesListView.TabIndex = 2;
            this.SentMessagesListView.UseCompatibleStateImageBehavior = false;
            this.SentMessagesListView.View = System.Windows.Forms.View.Details;
            // 
            // SentTime
            // 
            this.SentTime.Text = "Time";
            this.SentTime.Width = 80;
            // 
            // SentHeader
            // 
            this.SentHeader.Text = "Header";
            this.SentHeader.Width = 130;
            // 
            // SentMessage
            // 
            this.SentMessage.Text = "Message";
            this.SentMessage.Width = 400;
            // 
            // ReceivedMessagesListView
            // 
            this.ReceivedMessagesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ReceivedTime,
            this.ReceivedHeader,
            this.ReceivedMessage});
            this.ReceivedMessagesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReceivedMessagesListView.Location = new System.Drawing.Point(0, 0);
            this.ReceivedMessagesListView.MultiSelect = false;
            this.ReceivedMessagesListView.Name = "ReceivedMessagesListView";
            this.ReceivedMessagesListView.Size = new System.Drawing.Size(488, 300);
            this.ReceivedMessagesListView.TabIndex = 1;
            this.ReceivedMessagesListView.UseCompatibleStateImageBehavior = false;
            this.ReceivedMessagesListView.View = System.Windows.Forms.View.Details;
            // 
            // ReceivedTime
            // 
            this.ReceivedTime.Text = "Time";
            this.ReceivedTime.Width = 80;
            // 
            // ReceivedHeader
            // 
            this.ReceivedHeader.Text = "Header";
            this.ReceivedHeader.Width = 130;
            // 
            // ReceivedMessage
            // 
            this.ReceivedMessage.Text = "Message";
            this.ReceivedMessage.Width = 400;
            // 
            // ReceivedMessagesLbl
            // 
            this.ReceivedMessagesLbl.AutoSize = true;
            this.ReceivedMessagesLbl.Location = new System.Drawing.Point(3, 0);
            this.ReceivedMessagesLbl.Name = "ReceivedMessagesLbl";
            this.ReceivedMessagesLbl.Size = new System.Drawing.Size(104, 13);
            this.ReceivedMessagesLbl.TabIndex = 0;
            this.ReceivedMessagesLbl.Text = "Messages Received";
            // 
            // ChatServerStatusLbl
            // 
            this.ChatServerStatusLbl.AutoSize = true;
            this.ChatServerStatusLbl.Location = new System.Drawing.Point(75, 44);
            this.ChatServerStatusLbl.Name = "ChatServerStatusLbl";
            this.ChatServerStatusLbl.Size = new System.Drawing.Size(69, 13);
            this.ChatServerStatusLbl.TabIndex = 5;
            this.ChatServerStatusLbl.Text = "Server offline";
            // 
            // ChatServerLbl
            // 
            this.ChatServerLbl.AutoSize = true;
            this.ChatServerLbl.Location = new System.Drawing.Point(3, 44);
            this.ChatServerLbl.Name = "ChatServerLbl";
            this.ChatServerLbl.Size = new System.Drawing.Size(66, 13);
            this.ChatServerLbl.TabIndex = 6;
            this.ChatServerLbl.Text = "Chat Server:";
            // 
            // GameServerLabel
            // 
            this.GameServerLabel.AutoSize = true;
            this.GameServerLabel.Location = new System.Drawing.Point(3, 24);
            this.GameServerLabel.Name = "GameServerLabel";
            this.GameServerLabel.Size = new System.Drawing.Size(72, 13);
            this.GameServerLabel.TabIndex = 7;
            this.GameServerLabel.Text = "Game Server:";
            // 
            // GameServerStatusLbl
            // 
            this.GameServerStatusLbl.AutoSize = true;
            this.GameServerStatusLbl.Location = new System.Drawing.Point(75, 24);
            this.GameServerStatusLbl.Name = "GameServerStatusLbl";
            this.GameServerStatusLbl.Size = new System.Drawing.Size(69, 13);
            this.GameServerStatusLbl.TabIndex = 8;
            this.GameServerStatusLbl.Text = "Server offline";
            // 
            // ViewGameServerClientsBtn
            // 
            this.ViewGameServerClientsBtn.Enabled = false;
            this.ViewGameServerClientsBtn.Location = new System.Drawing.Point(209, 14);
            this.ViewGameServerClientsBtn.Name = "ViewGameServerClientsBtn";
            this.ViewGameServerClientsBtn.Size = new System.Drawing.Size(75, 23);
            this.ViewGameServerClientsBtn.TabIndex = 9;
            this.ViewGameServerClientsBtn.Text = "View Clients";
            this.ViewGameServerClientsBtn.UseVisualStyleBackColor = true;
            this.ViewGameServerClientsBtn.Click += new System.EventHandler(this.ViewGameServerClientsBtn_Click);
            // 
            // ViewChatServerClientsBtn
            // 
            this.ViewChatServerClientsBtn.Enabled = false;
            this.ViewChatServerClientsBtn.Location = new System.Drawing.Point(209, 39);
            this.ViewChatServerClientsBtn.Name = "ViewChatServerClientsBtn";
            this.ViewChatServerClientsBtn.Size = new System.Drawing.Size(75, 23);
            this.ViewChatServerClientsBtn.TabIndex = 10;
            this.ViewChatServerClientsBtn.Text = "View Clients";
            this.ViewChatServerClientsBtn.UseVisualStyleBackColor = true;
            this.ViewChatServerClientsBtn.Click += new System.EventHandler(this.ViewChatServerClientsBtn_Click);
            // 
            // SentMessagesLbl
            // 
            this.SentMessagesLbl.AutoSize = true;
            this.SentMessagesLbl.Location = new System.Drawing.Point(3, 2);
            this.SentMessagesLbl.Name = "SentMessagesLbl";
            this.SentMessagesLbl.Size = new System.Drawing.Size(80, 13);
            this.SentMessagesLbl.TabIndex = 3;
            this.SentMessagesLbl.Text = "Messages Sent";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer1.Size = new System.Drawing.Size(1003, 335);
            this.splitContainer1.SplitterDistance = 492;
            this.splitContainer1.TabIndex = 11;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.ReceivedMessagesLbl);
            this.splitContainer3.Panel1MinSize = 15;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.ReceivedMessagesListView);
            this.splitContainer3.Size = new System.Drawing.Size(488, 331);
            this.splitContainer3.SplitterDistance = 27;
            this.splitContainer3.TabIndex = 12;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.SentMessagesLbl);
            this.splitContainer4.Panel1MinSize = 15;
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.SentMessagesListView);
            this.splitContainer4.Size = new System.Drawing.Size(503, 331);
            this.splitContainer4.SplitterDistance = 27;
            this.splitContainer4.TabIndex = 12;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitter1);
            this.splitContainer2.Panel1.Controls.Add(this.GameServerLabel);
            this.splitContainer2.Panel1.Controls.Add(this.ChatServerLbl);
            this.splitContainer2.Panel1.Controls.Add(this.ClientsPanel);
            this.splitContainer2.Panel1.Controls.Add(this.ViewGameServerClientsBtn);
            this.splitContainer2.Panel1.Controls.Add(this.ViewChatServerClientsBtn);
            this.splitContainer2.Panel1.Controls.Add(this.ChatServerStatusLbl);
            this.splitContainer2.Panel1.Controls.Add(this.GameServerStatusLbl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(1003, 650);
            this.splitContainer2.SplitterDistance = 311;
            this.splitContainer2.TabIndex = 12;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 307);
            this.splitter1.TabIndex = 11;
            this.splitter1.TabStop = false;
            // 
            // ServerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 674);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.ServerMenuStrip);
            this.Name = "ServerUI";
            this.Text = "XNA RTS Server";
            this.Load += new System.EventHandler(this.ServerUI_Load);
            this.ServerMenuStrip.ResumeLayout(false);
            this.ServerMenuStrip.PerformLayout();
            this.ClientsPanel.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel1.PerformLayout();
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ServerMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ServerMenuTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.Label ClientsLbl;
        private System.Windows.Forms.Panel ClientsPanel;
        private System.Windows.Forms.Label ReceivedMessagesLbl;
        public System.Windows.Forms.ListView ClientsListView;
        public System.Windows.Forms.ListView ReceivedMessagesListView;
        public System.Windows.Forms.Label ChatServerStatusLbl;
        private System.Windows.Forms.Label ChatServerLbl;
        private System.Windows.Forms.Label GameServerLabel;
        private System.Windows.Forms.ToolStripMenuItem StartServerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopServerMenuItem;
        public System.Windows.Forms.ToolStripMenuItem StartGameServer;
        public System.Windows.Forms.ToolStripMenuItem StartChatServer;
        public System.Windows.Forms.ToolStripMenuItem StopGameServer;
        public System.Windows.Forms.ToolStripMenuItem StopChatServer;
        public System.Windows.Forms.Label GameServerStatusLbl;
        public System.Windows.Forms.Button ViewGameServerClientsBtn;
        public System.Windows.Forms.Button ViewChatServerClientsBtn;
        private System.Windows.Forms.ColumnHeader IP;
        private System.Windows.Forms.ColumnHeader Username;
        public System.Windows.Forms.ListView SentMessagesListView;
        private System.Windows.Forms.Label SentMessagesLbl;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ColumnHeader SentMessage;
        private System.Windows.Forms.ColumnHeader ReceivedMessage;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.ColumnHeader SentTime;
        private System.Windows.Forms.ColumnHeader SentHeader;
        private System.Windows.Forms.ColumnHeader ReceivedTime;
        private System.Windows.Forms.ColumnHeader ReceivedHeader;
    }
}

