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
        public string meuIp { get; set; } = "teste";
        private SocketClient client;
        private SocketServer server;

        public Chat()
        {
            InitializeComponent();

            BindingContext = this;


            enviarMensagem = new Command(_enviarMensagem);
            button.Command = enviarMensagem;

            server = new SocketServer();
            Task.Run(() => {                
                    RunServer();                
            });


            client = new SocketClient();
            meuIp = client.myIpAddress.ToString();

            
            //RunServer();

        }

        public void AddMensagem(View conteudo, Mensagem.Remetente remetente)
        {
            var mensagem = new Mensagem(conteudo, remetente);
            stack.Children.Add(mensagem);
            scrollView.ScrollToAsync(mensagem, ScrollToPosition.End, false);
        }

        public void MensagemRecebida(string msg)
        {
            Device.BeginInvokeOnMainThread(() => {
                AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Cliente);
            });
        }

        private void StartConnection(string ipText)
        {
            client.StartClient(IPAddress.Parse(ipText));
        }

        public async void RunServer()
        {            
            server.Listen(MensagemRecebida);
        }

        private void _enviarMensagem()
        {
            if (editor.Text == String.Empty) return;
            string msg = editor.Text;
            editor.Text = "";
            Console.WriteLine(msg);

            
           
            if(client._Client.Connected){
                AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Servidor);
                client.SendMessage(msg);
            }
            else
            {
                AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Servidor);
                StartConnection(msg);
                if (client._Client.Connected)
                {
                    AddMensagem(new Label() { Text = "Conectado com sucesso" }, Mensagem.Remetente.Cliente);
                }
            }
        }


    }
}