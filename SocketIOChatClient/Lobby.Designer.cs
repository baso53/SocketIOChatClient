namespace SocketIOChatClient
{
    partial class Lobby
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.requestRoomListBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.addRoomBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.switchToRoomBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.roomListView = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RoomName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Password = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UserCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusBarLabel
            // 
            this.statusBarLabel.ForeColor = System.Drawing.Color.Red;
            this.statusBarLabel.Name = "statusBarLabel";
            this.statusBarLabel.Size = new System.Drawing.Size(88, 17);
            this.statusBarLabel.Text = "Not Connected";
            // 
            // requestRoomListBackgroundWorker
            // 
            //this.requestRoomListBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.requestRoomListBackgroundWorker_DoWork);
            //this.requestRoomListBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.requestRoomListBackgroundWorker_RunWorkerCompleted);
            // 
            // addRoomBackgroundWorker
            // 
            this.addRoomBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addRoomBackgroundWorker_DoWork);
            this.addRoomBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.addRoomBackgroundWorker_RunWorkerCompleted);
            // 
            // switchToRoomBackgroundWorker
            // 
            this.switchToRoomBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.switchToRoomBackgroundWorker_DoWork);
            this.switchToRoomBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.switchToRoomBackgroundWorker_RunWorkerCompleted);
            // 
            // roomListView
            // 
            this.roomListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.roomListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.roomListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.roomListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.RoomName,
            this.Password,
            this.UserCount});
            this.roomListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roomListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roomListView.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.roomListView.FullRowSelect = true;
            this.roomListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.roomListView.Location = new System.Drawing.Point(0, 0);
            this.roomListView.Margin = new System.Windows.Forms.Padding(0);
            this.roomListView.MultiSelect = false;
            this.roomListView.Name = "roomListView";
            this.roomListView.Size = new System.Drawing.Size(800, 428);
            this.roomListView.TabIndex = 7;
            this.roomListView.UseCompatibleStateImageBehavior = false;
            this.roomListView.View = System.Windows.Forms.View.Details;
            this.roomListView.ItemActivate += new System.EventHandler(this.roomListView_ItemActivate);
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 50;
            // 
            // RoomName
            // 
            this.RoomName.Text = "Room Name";
            this.RoomName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RoomName.Width = 250;
            // 
            // Password
            // 
            this.Password.Text = "Password Protected";
            this.Password.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Password.Width = 180;
            // 
            // UserCount
            // 
            this.UserCount.Text = "Users Connected";
            this.UserCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.UserCount.Width = 320;
            // 
            // Lobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.Controls.Add(this.roomListView);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Lobby";
            this.Size = new System.Drawing.Size(800, 450);
            this.Load += new System.EventHandler(this.Lobby_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.ComponentModel.BackgroundWorker requestRoomListBackgroundWorker;
        private System.Windows.Forms.ToolStripStatusLabel statusBarLabel;
        private System.ComponentModel.BackgroundWorker addRoomBackgroundWorker;
        private System.ComponentModel.BackgroundWorker switchToRoomBackgroundWorker;
        private System.Windows.Forms.ListView roomListView;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader RoomName;
        private System.Windows.Forms.ColumnHeader Password;
        private System.Windows.Forms.ColumnHeader UserCount;
    }
}
