using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Forms.PlatformConfiguration;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public ICommand enviarMensagem { get; }
        public ICommand anexar_item { get; }
        public string meuIp { get; set; } = "teste";
        private SocketClient client;
        private SocketServer server;

        public Chat()
        {
            InitializeComponent();

            BindingContext = this;


            enviarMensagem = new Command(_enviarMensagem);
            anexar_item = new Command(_enviarImagem);
            button.Command = enviarMensagem;

            server = new SocketServer();
            Task.Run(() => {                
                    RunServer();                
            });


            client = new SocketClient();
            meuIp = client.myIpAddress.ToString();
            //RunServer();

        }

        public void ShowMensagem(string mensagem, TipoMensagem tipo, Remetente remetente, int status = 1)
        {
            var msg = Mensagem.CriarMensagem(mensagem, tipo, remetente, status);
            stack.Children.Add(msg);
            scrollView.ScrollToAsync(msg, ScrollToPosition.End, false);
        }

        public void MensagemRecebida(string msg, TipoMensagem tipo)
        {
            Device.BeginInvokeOnMainThread(() => {
                ShowMensagem(msg, tipo, Remetente.CLIENTE);
            });
        }

        private void StartConnection(string ipText)
        {
            try
            {
                client.StartClient(IPAddress.Parse(ipText));
            }catch(Exception e)
            {
                Device.BeginInvokeOnMainThread(() => {
                    ShowMensagem("IP invalido, seu burro.", TipoMensagem.TEXTO, Remetente.CLIENTE, 0);
                });
            }
        }

        public async void RunServer()
        {            
            server.Listen(MensagemRecebida);            
            client.StopClient();
            Device.BeginInvokeOnMainThread(() => {
                ShowMensagem("Desconectado", TipoMensagem.TEXTO, Remetente.CLIENTE, 0);
            });
        }

        private void _enviarMensagem()
        {
            if (editor.Text == string.Empty) return;
            string msg = editor.Text;
            editor.Text = "";
            
           
            if(client._Client.Connected){
                ShowMensagem(msg, TipoMensagem.TEXTO, Remetente.SERVIDOR);
                client.SendMessage(msg);
            }
            else
            {
                ShowMensagem(msg, TipoMensagem.TEXTO, Remetente.SERVIDOR);
                StartConnection(msg);
                if (client._Client.Connected)
                {
                    ShowMensagem("Conectado com sucesso", TipoMensagem.TEXTO, Remetente.SERVIDOR);
                    
                }
                
            }
        }

        private void _enviarImagem()
        {
            string imgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures), "display.png");

            using (StreamReader sr = new StreamReader(imgPath))
            {
                BinaryReader binreader = new BinaryReader(sr.BaseStream);
                var allData = ReadAllBytes(binreader);
                //Bitmap bitmap = BitmapFactory.DecodeByteArray(allData, 0, allData.Length);
                client.SendImage(allData);
            }           

        }

        public static byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }

    }
}