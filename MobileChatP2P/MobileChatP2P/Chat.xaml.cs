using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public ICommand enviarMensagem { get; }
        public string meuIp { get; set; } = "192.168.0.0.1";
        private SocketClient client;
        private SocketServer server;

        public Chat()
        {
            InitializeComponent();

            BindingContext = this;


            enviarMensagem = new Command(_enviarMensagem);
            button.Command = enviarMensagem;

            server = new SocketServer();
            Thread listenThread = new Thread(RunServer);
            listenThread.Start();
            //RunServer();

        }

        public void AddMensagem(View conteudo, Mensagem.Remetente remetente)
        {
            stack.Children.Add(new Mensagem(conteudo, remetente));
        }

        public void MensagemRecebida(string msg)
        {
            AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Servidor);
        }

        private void StartConnection(string ipText)
        {
            client.StartClient(IPAddress.Parse(ipText));
        }

        public async void RunServer()
        {
            
            var msg = await server.Listen(MensagemRecebida);
            AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Cliente);
        }

        private void _enviarMensagem()
        {
            string msg = editor.Text;
            Console.WriteLine(msg);
            AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Servidor);
        }


    }
}