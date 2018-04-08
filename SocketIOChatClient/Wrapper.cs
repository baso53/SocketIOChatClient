using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.Threading;
using System.Windows.Forms;


namespace SocketIOChatClient
{
    public partial class Wrapper : Form
    {
        //public static Socket socket = IO.Socket("http://192.168.42.7:3000");
        public static Socket socket = IO.Socket("wss://socketiochatserver.azurewebsites.net/");
        private Lobby lobby = null;
        private Room room = null;
        private Thread reconnectThread = null;
        delegate void ReconnectDelegate();

        public Wrapper()
        {
            InitializeComponent();
            this.Controls.Add(lobby = new Lobby());

            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                this.reconnectThread = new Thread(() => ReconnectThreadSafe());
                reconnectThread.Start();

                MessageBox.Show("Error communicating with server! Returning to Lobby.");
            });
            
        }

        private void ReconnectThreadSafe ()
        {
            this.Reconnect();
        }

        private void Reconnect()
        {
            if (this.InvokeRequired)
            {
                ReconnectDelegate deleg = new ReconnectDelegate(ReconnectThreadSafe);
                this.Invoke(deleg, new object[] { });
            }
            else
            {
                this.Controls.Clear();
                if (lobby != null && !lobby.IsDisposed)
                {
                    lobby.Dispose();
                }
                if (room != null && !room.IsDisposed)
                {
                    room.Dispose();
                }
                this.Controls.Add(lobby = new Lobby());
            }
        }

        public void switchToRoom(JObject _room, JObject connectedUsers)
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
