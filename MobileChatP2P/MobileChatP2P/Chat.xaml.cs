using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android.Graphics;
using System.IO;
using Java.IO;

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

        public void AddMensagem(View conteudo, Mensagem.Remetente remetente, int status = 1)
        {
            var mensagem = new Mensagem(conteudo, remetente, status);
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
            try
            {
                client.StartClient(IPAddress.Parse(ipText));
            }catch(Exception e)
            {
                Device.BeginInvokeOnMainThread(() => {
                    AddMensagem(new Label() { Text = "IP invalido, seu burro." }, Mensagem.Remetente.Cliente, 0);
                });
            }
        }

        public async void RunServer()
        {            
            server.Listen(MensagemRecebida);            
            client.StopClient();
            Device.BeginInvokeOnMainThread(() => {
                AddMensagem(new Label() { Text = "Desconectado" }, Mensagem.Remetente.Cliente, 0);
            });
        }

        private void _enviarMensagem()
        {
            if (editor.Text == String.Empty) return;
            string msg = editor.Text;
            editor.Text = "";
            
           
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
                _enviarImagem();
                //_enviarVideo();
            }
        }

        private void _enviarImagem()
        {
            string imgPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "ylderlan.jpg");

            /*using (StreamReader sr = new StreamReader(imgPath))
            {
                BinaryReader binreader = new BinaryReader(sr.BaseStream);
                var allData = ReadAllBytes(binreader);
                //Bitmap bitmap = BitmapFactory.DecodeByteArray(allData, 0, allData.Length);
                client.SendImage(allData);
            }     */
            byte[] allData;
            allData = System.IO.File.ReadAllBytes(imgPath);
            client.SendImage(allData);
        }

        private void _enviarVideo()
        {
            string videoPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "batata.mp4");

            /*using (StreamReader sr = new StreamReader(imgPath))
            {
                BinaryReader binreader = new BinaryReader(sr.BaseStream);
                var allData = ReadAllBytes(binreader);
                //Bitmap bitmap = BitmapFactory.DecodeByteArray(allData, 0, allData.Length);
                client.SendImage(allData);
            }     */
            byte[] allData;
            allData = System.IO.File.ReadAllBytes(videoPath);
            client.SendVideo(allData);
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