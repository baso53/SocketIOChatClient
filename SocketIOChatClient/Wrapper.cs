using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SocketIOChatClient
{
    public partial class Wrapper : Form
    {
        public static Socket socket = IO.Socket("http://192.168.42.7:3000");
        private Lobby lobby;
        private Room room;

        public Wrapper()
        {
            InitializeComponent();
            this.Controls.Add(lobby = new Lobby());
        }

        public void switchToRoom(JObject _room, JArray connectedUsers)
        {
            lobby.Dispose();
            this.Controls.Clear();
            this.Controls.Add(room = new Room(_room, connectedUsers));
        }

        public void switchToLobby()
        {
            room.Dispose();
            this.Controls.Clear();
            this.Controls.Add(lobby = new Lobby());
        }
    }
}
