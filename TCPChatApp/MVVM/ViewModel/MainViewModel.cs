using System.Collections.ObjectModel;
using System.Windows;
using TCPChatApp.MVVM.Core;
using TCPChatApp.MVVM.Model;
using TCPChatApp.Net;

namespace TCPChatApp.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        private Server server;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            server = new Server();
            server.ConnectedEvent += UserConnected;
            server.MessageReceivedEvent += MessageReceived;
            server.DisonnectedEvent += UserDisconnected;
            ConnectToServerCommand = new RelayCommand(o => server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => server.SendMSGToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        private void UserDisconnected()
        {
            var uid = server.packetReader.ReadMSG();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = server.packetReader.ReadMSG();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = server.packetReader.ReadMSG(),
                UID = server.packetReader.ReadMSG(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
