using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace SocketIOChatClient
{
    public partial class Room : UserControl
    {
        public JObject room;
        public JObject userListInitial;
        private Thread messageThread = null;
        private Thread leaveThread = null;
        private Thread userListThread = null;
        private Thread populateMessagesThread = null;
        delegate void MessageDelegate(JObject msg);
        delegate void LeaveRoomVoidDelegate();
        delegate void ConnectedusersDelegate(JObject users);
        
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

        public Room(JObject _room, JObject userListInitial)
        {
            InitializeComponent();
            room = _room;
            this.roomNameLabel.Text = room["name"].ToString();
            this.statusBarLabel.Text = "Connected";
            this.statusBarLabel.ForeColor = System.Drawing.Color.Green;

            this.userListThread = new Thread(() => UserListThreadSafe(userListInitial));
            this.userListThread.Start();

            this.PopulateMessages();

            Wrapper.socket.On("chat-message", (msg) =>
            {
                this.messageThread = new Thread(() => MessageThreadSafe((JObject)msg));
                this.messageThread.Start();
            });

            Wrapper.socket.On("room-deleted", () => {
                this.leaveThread = new Thread(() => LeaveRoomSafe());
                this.leaveThread.Start();
                MessageBox.Show("Room deleted by admin.");
            });

            Wrapper.socket.On("users-list", (userList) =>
            {
                this.userListThread = new Thread(() => UserListThreadSafe((JObject)userList));
                this.userListThread.Start();
            });
        }

        private void Room_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text != "")
            {
                JObject msg = new JObject();
                msg["message"] = inputTextBox.Text;

                Wrapper.socket.Emit("chat-message", (ack) =>
                {
                    JObject response = (JObject)ack;

                    if (!response["success"].ToObject<bool>())
                    {
                        MessageBox.Show(response["error"].ToString());
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

        private void leaveButton_Click(object sender, EventArgs e)
        {
            Wrapper.socket.Emit("room-leave", (ack) =>
            {
                JObject response = (JObject)ack;

                if (response["success"].ToObject<bool>())
                {
                    this.leaveThread = new Thread(() => LeaveRoomSafe());
                    this.leaveThread.Start();
                }
                else
                {
                    MessageBox.Show(response["error"].ToString());
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
                JObject response = (JObject)ack;

                if (!response["success"].ToObject<bool>())
                {
                    MessageBox.Show(response["error"].ToString());
                }
            }, masterPassword);
        }
    }
}


namespace SocketIOChatClient
{
    partial class Room
    {
        private void UserListThreadSafe(JObject userList)
        {
            this.populateUserList(userList);
        }

        private void populateUserList(JObject userList)
        {
            if (this.userListView.InvokeRequired)
            {
                ConnectedusersDelegate deleg = new ConnectedusersDelegate(UserListThreadSafe);
                this.Invoke(deleg, new object[] { userList });
            }
            else
            {
                userListView.Clear();

                foreach (JValue user in userList["connectedUsers"])
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
                MessageDelegate deleg = new MessageDelegate(AddMessage);
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

        private void PopulateMessages()
        {
            Wrapper.socket.Emit("request-chat-history", (ack) =>
            {
                JObject response = (JObject)ack;
                if (response["success"].ToObject<bool>())
                {
                    this.populateMessagesThread = new Thread(() => PopulateMessagesThreadSafe(response));
                    this.populateMessagesThread.Start();
                }
                else
                {
                    MessageBox.Show(response["error"].ToString());
                }
            }, this.room);
        }

        private void PopulateMessagesThreadSafe(JObject messages)
        {
            this.Populate(messages);
        }

        private void Populate(JObject messages)
        {
            if (this.InvokeRequired)
            {
                MessageDelegate deleg = new MessageDelegate(PopulateMessagesThreadSafe);
                this.Invoke(deleg, new object[] { messages });
            }
            else
            {
                foreach (JObject message in messages["messages"])
                {
                    MessageAppender.FormatMessage(messagesTextBox, message);
                }
                messagesTextBox.ScrollToCaret();
            }
        }
    }
}