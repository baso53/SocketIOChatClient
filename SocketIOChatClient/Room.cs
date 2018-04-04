using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace SocketIOChatClient
{
    public partial class Room : UserControl
    {
        private JObject room;
        private Thread messageThread = null;
        private Thread leaveThread = null;
        private Thread userListThread = null;
        delegate void StringArgReturningVoidDelegate(JObject msg);
        delegate void LeaveRoomVoidDelegate();
        delegate void ConnectedusersDelegate(JArray users);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Wrapper.socket.Off("chat-message");
                Wrapper.socket.Off("room-deleted");
                Wrapper.socket.Off("users-list");
            }

            base.Dispose(disposing);
        }

        public Room(JObject _room, JArray userList)
        {
            InitializeComponent();
            room = _room;
            this.roomNameLabel.Text = room["name"].ToString();
            this.statusBarLabel.Text = "Connected";
            this.statusBarLabel.ForeColor = System.Drawing.Color.Green;
            this.addMessageBackgroundWorker.WorkerReportsProgress = true;

            this.userListThread = new Thread(() => UserListThreadSafe((JArray)userList));
            this.userListThread.Start();
        }

        private void Room_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
            
            this.addMessageBackgroundWorker.RunWorkerAsync();
            
            Wrapper.socket.On("chat-message", (msg) =>
            {
                this.messageThread = new Thread(() => MessageThreadSafe((JObject) msg));
                this.messageThread.Start();
            });

            Wrapper.socket.On("room-deleted", () => {
                this.leaveThread = new Thread(() => LeaveRoomSafe());
                this.leaveThread.Start();
                MessageBox.Show("Room deleted by admin.");
            });

            Wrapper.socket.On("users-list", (userList) =>
            {
                this.userListThread = new Thread(() => UserListThreadSafe((JArray)userList));
                this.userListThread.Start();
            });
        }

        private void UserListThreadSafe(JArray userList)
        {
            this.populateUserList(userList);
        }

        private void populateUserList(JArray userList)
        {
            if (this.userListView.InvokeRequired)
            {
                ConnectedusersDelegate deleg = new ConnectedusersDelegate(UserListThreadSafe);
                this.Invoke(deleg, new object[] { userList });
            }
            else
            {
                userListView.Clear();
                
                foreach (JValue user in userList)
                {
                    userListView.Items.Add(new ListViewItem(user.ToString()));
                }
            }
        }

        private void MessageThreadSafe(JObject msg)
        {
            this.AddMessage(msg);
        }

        private void AddMessage(JObject msg)
        {
            if (this.messagesTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate deleg = new StringArgReturningVoidDelegate(AddMessage);
                this.Invoke(deleg, new object[] { msg });
            }
            else
            {
                MessageAppender.FormatMessage(messagesTextBox, msg);
                messagesTextBox.ScrollToCaret();
            }
        }

        private void LeaveRoomSafe()
        {
            this.LeaveRoom();
        }

        private void LeaveRoom()
        {
            if (this.InvokeRequired)
            {
                LeaveRoomVoidDelegate deleg = new LeaveRoomVoidDelegate(LeaveRoom);
                this.Invoke(deleg, new object[] { });
            }
            else
            {
                ((Wrapper)this.Parent).switchToLobby();
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text != "")
            {
                var msg = new JObject();
                msg["message"] = inputTextBox.Text;

                Wrapper.socket.Emit("chat-message", (ack) =>
                {
                    if (!((JObject)ack)["success"].ToObject<bool>())
                    {
                        MessageBox.Show(((JObject)ack)["error"].ToObject<string>());
                    }
                }, msg);
                inputTextBox.Text = "";
            }
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendButton_Click(this, new EventArgs());
                e.SuppressKeyPress = true;
            }
        }

        private void addMessageBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Wrapper.socket.Emit("request-chat-history", (ack) =>
            {
                e.Result = (JObject)ack;
            }, room);
            while (e.Result == null) ;
        }

        private void addMessageBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var msg = (JObject)e.Result;
            if (msg["success"].ToObject<bool>())
            {
                foreach (JObject message in msg["results"])
                {
                    MessageAppender.FormatMessage(messagesTextBox, message);
                }
            }
            messagesTextBox.ScrollToCaret();
        }
        
        private void leaveButton_Click(object sender, EventArgs e)
        {
            Wrapper.socket.Emit("room-leave", (ack) =>
            {
                if (((JObject)ack)["success"].ToObject<bool>())
                {
                    this.leaveThread = new Thread(() => LeaveRoomSafe());
                    this.leaveThread.Start();
                }
                else
                {
                    MessageBox.Show("Error leaving the room!");
                }
            });
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string masterPassword = Prompt.ShowDialog("Master Password:", "Type the password for deleting the room:", 18, true);
            while (masterPassword.Length == 0)
            {
                masterPassword = Prompt.ShowDialog("Master Password:", "Type the password for deleting the room:", 18, true);
            }
            if (masterPassword == "=?*exitcode")
            {
                return;
            }

            Wrapper.socket.Emit("room-delete", (ack) =>
            {
                if (!((JObject)ack)["success"].ToObject<bool>())
                {
                    MessageBox.Show(((JObject)ack)["error"].ToString());
                }
            }, masterPassword);
        }
    }
}
