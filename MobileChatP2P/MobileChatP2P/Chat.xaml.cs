using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public string meuIp { get; set; } = "192.168.0.0.1";
        private SocketClient client;
        private SocketServer server;
        public Chat()
        {
            InitializeComponent();

            BindingContext = this;

            
           

            //client = new SocketClient();

            //meuIp = client.myIpAddress.ToString();
            RunServer();
            //StartConnection("10.0.0.105");
            //client.SendMessage("Eduardo eh doidao<EOF>");
        }

        private void StartConnection(string ipText)
        {
            client.StartClient(IPAddress.Parse(ipText));
        }

        public async void RunServer()
        {
            server = new SocketServer();
            meuIp = await server.StartServer();
        }
    }
}