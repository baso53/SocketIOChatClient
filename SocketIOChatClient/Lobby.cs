using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace SocketIOChatClient
{
    public partial class Lobby : UserControl
    {
        public Lobby()
        {
            InitializeComponent();
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
            this.requestRoomListBackgroundWorker.RunWorkerAsync();
        }

        private void roomListView_ItemActivate(object sender, EventArgs e)
        {
            if (this.roomListView.SelectedItems[0].Index == 0)
            {
                addNewRoom();
            }
            else
            {
                logInToRoom();
            }
        }

        private void addNewRoom()
        {
            string name = Prompt.ShowDialog("Name:", "What do you want the room to be called?", 50, false);
            while (name.Length == 0)
            {
                name = Prompt.ShowDialog("Name:", "What do you want the room to be called?", 50, false);
            }
            if (name == "=?*exitcode")
            {
                return;
            }

            string password = Prompt.ShowDialog("Password:", "(Optional) Type a password for entering the room:", 18, true);
            if (password == "=?*exitcode")
            {
                return;
            }

            string masterPassword = Prompt.ShowDialog("Master Password:", "Type a password for deleting the room:", 18, true);
            while (masterPassword.Length == 0)
            {
                masterPassword = Prompt.ShowDialog("Master Password:", "Type a password for deleting the room:", 18, true);
            }
            if (masterPassword == "=?*exitcode")
            {
                return;
            }

            JObject newRoom = new JObject();
            newRoom["name"] = name;
            newRoom["password"] = password;
            newRoom["masterPassword"] = masterPassword;
            this.addRoomBackgroundWorker.RunWorkerAsync(newRoom);
        }

        private void logInToRoom()
        {
            JObject selectedRoom = (JObject)this.roomListView.SelectedItems[0].Tag;

            string username = Prompt.ShowDialog("Username:", "Select a username:", 18, false);
            while (username.Length == 0)
            {
                username = Prompt.ShowDialog("Username:", "Select a username:", 18, false);
            }
            if (username == "=?*exitcode")
            {
                return;
            }

            string password = "";
            if (((JObject)roomListView.SelectedItems[0].Tag)["passwordProtected"].ToObject<string>() == "Yes")
            {
                password = Prompt.ShowDialog("Password:", "Type in the password for the room:", 18, true);
                while (password.Length == 0)
                {
                    password = Prompt.ShowDialog("Password:", "Type in the password for the room:", 18, true);
                }
                if (password == "=?*exitcode")
                {
                    return;
                }
            }

            selectedRoom["username"] = username;
            selectedRoom["password"] = password;
            this.switchToRoomBackgroundWorker.RunWorkerAsync(selectedRoom);
        }

        private void requestRoomListBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Wrapper.socket.Emit("request-room-list", (ack) =>
            {
                if (((JObject)ack)["success"].ToObject<bool>())
                {
                    e.Result = (JObject)ack;
                }
                else
                {
                    MessageBox.Show(((JObject)ack)["error"].ToObject<string>());
                    e.Result = (JObject)ack;
                }
            });
            while (e.Result == null) ;
        }

        private void requestRoomListBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (((JObject)e.Result)["success"].ToObject<bool>())
            {
                roomListView.Items.Clear();
                ListViewItem addNewRoomRow = new ListViewItem();
                addNewRoomRow.Text = "+";
                addNewRoomRow.SubItems.Add("ADD A NEW ROOM");
                this.roomListView.Items.Add(addNewRoomRow);

                this.statusBarLabel.Text = "Connected";
                this.statusBarLabel.ForeColor = System.Drawing.Color.Green;

                IList<JToken> results = ((JToken)e.Result)["roomList"].Children().ToList();
                foreach (JToken result in results)
                {
                    ListViewItem row = new ListViewItem();
                    row.Text = result["id"].ToObject<String>();
                    row.SubItems.Add(result["name"].ToObject<String>());
                    row.SubItems.Add(result["passwordProtected"].ToObject<String>());
                    row.SubItems.Add(result["numberOfClients"].ToObject<String>());
                    row.Tag = result;

                    this.roomListView.Items.Add(row);
                }
            }
        }

        private void addRoomBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Wrapper.socket.Emit("add-room", (ack) =>
            {
                if (((JObject)ack)["success"].ToObject<bool>())
                {
                    e.Result = true;
                }
                else
                {
                    MessageBox.Show(((JObject)ack)["error"].ToObject<string>());
                    e.Result = false;
                }
            }, (JObject)e.Argument);
            while (e.Result == null) ;
        }

        private void addRoomBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                this.requestRoomListBackgroundWorker.RunWorkerAsync();
            }
        }

        private void switchToRoomBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Wrapper.socket.Emit("room-select", (ack) =>
            {
                if (((JObject)ack)["success"].ToObject<bool>())
                {
                    //((JArray)((JObject)ack)["connectedUsers"])
                    e.Result = (true, (JObject)e.Argument, ((JArray)((JObject)ack)["connectedUsers"]));
                }
                else
                {
                    MessageBox.Show(((JObject)ack)["error"].ToString());
                    e.Result = (false, (JObject)e.Argument, new JArray());
                }
            }, (JObject)e.Argument);
            while (e.Result == null) ;
        }

        private void switchToRoomBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool info, JObject room, JArray connectedUsers) result = ((bool, JObject, JArray)) e.Result;
            if (result.info)
            {
                result.room.Property("password").Remove();
                ((Wrapper)this.Parent).switchToRoom(result.room, result.connectedUsers);
            }
        }
    }
}