using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace SocketIOChatClient
{
    public partial class Lobby : UserControl
    {
        private Thread roomListThread = null;
        delegate void JObjectDelegate(JObject json);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Wrapper.socket.Off("room-list-changed");
            }

            base.Dispose(disposing);
        }

        public Lobby()
        {
            InitializeComponent();
            
            Wrapper.socket.Emit("request-room-list", (roomList) =>
            {
                this.roomListThread = new Thread(() => RoomListThreadSafe((JObject)roomList));
                this.roomListThread.Start();
            });

            Wrapper.socket.On("room-list-changed", () =>
            {
                Wrapper.socket.Emit("request-room-list", (roomList) =>
               {
                   this.roomListThread = new Thread(() => RoomListThreadSafe((JObject)roomList));
                   this.roomListThread.Start();
               });
            });
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        private void RoomListThreadSafe(JObject roomList)
        {
            this.PopulateRoomList(roomList);
        }

        private void PopulateRoomList (JObject response)
        {
            if (this.roomListView.InvokeRequired)
            {
                JObjectDelegate deleg = new JObjectDelegate(RoomListThreadSafe);
                this.Invoke(deleg, new object[] { response });
            }
            else
            {
                if (response["success"].ToObject<bool>())
                {
                    this.statusBarLabel.Text = "Connected";
                    this.statusBarLabel.ForeColor = System.Drawing.Color.Green;

                    roomListView.Items.Clear();
                    ListViewItem addNewRoomRow = new ListViewItem();
                    addNewRoomRow.Text = "+";
                    addNewRoomRow.SubItems.Add("ADD A NEW ROOM");
                    this.roomListView.Items.Add(addNewRoomRow);

                    foreach (JObject room in response["roomList"])
                    {
                        ListViewItem row = new ListViewItem();
                        row.Text = room["id"].ToString();
                        row.SubItems.Add(room["name"].ToString());
                        row.SubItems.Add(room["passwordProtected"].ToString());
                        row.SubItems.Add(room["numberOfClients"].ToString());
                        row.Tag = room;

                        this.roomListView.Items.Add(row);
                    }
                }
                else
                {
                    MessageBox.Show(response["error"].ToString());
                }
            }
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
            JObject selectedRoom = (JObject) this.roomListView.SelectedItems[0].Tag;

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
    }
}

namespace SocketIOChatClient
{
    partial class Lobby
    {
        //private void requestRoomListBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    Wrapper.socket.Emit("request-room-list", (ack) =>
        //    {
        //        JObject response = (JObject)ack;

        //        if (response["success"].ToObject<bool>())
        //        {
        //            e.Result = response;
        //        }
        //        else
        //        {
        //            MessageBox.Show(response["error"].ToString());
        //            e.Result = response;
        //        }
        //    });
        //    while (e.Result == null) ;
        //}

        //private void requestRoomListBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    JObject response = (JObject)e.Result;

        //    if (response["success"].ToObject<bool>())
        //    {
        //        this.statusBarLabel.Text = "Connected";
        //        this.statusBarLabel.ForeColor = System.Drawing.Color.Green;

        //        roomListView.Items.Clear();
        //        ListViewItem addNewRoomRow = new ListViewItem();
        //        addNewRoomRow.Text = "+";
        //        addNewRoomRow.SubItems.Add("ADD A NEW ROOM");
        //        this.roomListView.Items.Add(addNewRoomRow);

        //        foreach (JObject room in response["roomList"])
        //        {
        //            ListViewItem row = new ListViewItem();
        //            row.Text = room["id"].ToString();
        //            row.SubItems.Add(room["name"].ToString());
        //            row.SubItems.Add(room["passwordProtected"].ToString());
        //            row.SubItems.Add(room["numberOfClients"].ToString());
        //            row.Tag = room;

        //            this.roomListView.Items.Add(row);
        //        }
        //    }
        //}

        private void addRoomBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Wrapper.socket.Emit("add-room", (ack) =>
            {
                JObject response = (JObject)ack;

                if (response["success"].ToObject<bool>())
                {
                    e.Result = true;
                }
                else
                {
                    MessageBox.Show(response["error"].ToString());
                    e.Result = false;
                }
            }, (JObject)e.Argument);
            while (e.Result == null) ;
        }

        private void addRoomBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                Wrapper.socket.Emit("request-room-list", (roomList) =>
                {
                    this.roomListThread = new Thread(() => RoomListThreadSafe((JObject)roomList));
                    this.roomListThread.Start();
                });
            }
        }

        private void switchToRoomBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            JObject room = (JObject)e.Argument;

            Wrapper.socket.Emit("room-select", (ack) =>
            {
                JObject response = (JObject)ack;

                if (response["success"].ToObject<bool>())
                {
                    e.Result = (true, room, response);
                }
                else
                {
                    MessageBox.Show(response["error"].ToString());
                    e.Result = (false, new JObject(), new JObject());
                }
            }, room);
            while (e.Result == null) ;
        }

        private void switchToRoomBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool info, JObject room, JObject connectedUsers) result = ((bool, JObject, JObject))e.Result;
            if (result.info)
            {
                result.room.Property("password").Remove();
                ((Wrapper)this.Parent).switchToRoom(result.room, result.connectedUsers);
            }
        }
    }
}