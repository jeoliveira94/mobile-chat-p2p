using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Forms.PlatformConfiguration;
using Plugin.Media;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public ICommand enviarMensagem { get; }
        public ICommand anexar_imagem { get; }
        public ICommand anexar_video { get; }
        public string meuIp { get; set; } = "teste";
        private SocketClient client;
        private SocketServer server;

        public Chat()
        {
            InitializeComponent();

            BindingContext = this;


            enviarMensagem = new Command(_enviarMensagem);
            anexar_imagem = new Command(async () => {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,

                });

                if (file == null)
                    return;
                _enviarImagem(file.Path);

                file.Dispose();
                
            });

            anexar_video = new Command(async () => {
                if (!CrossMedia.Current.IsPickVideoSupported)
                {
                    DisplayAlert("Videos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                /*var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,

                });*/

                var file = await Plugin.Media.CrossMedia.Current.PickVideoAsync();

                if (file == null)
                    return;
                
                _enviarVideo(file.Path);
                file.Dispose();
            });

            button.Command = enviarMensagem;
            btn_Anexar_Imagem.Command = anexar_imagem;
            btn_Anexar_Video.Command = anexar_video;

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

                //_enviarImagem();
                //_enviarVideo();

            }
        }

        private void _enviarImagem(string imgPath)
        {

            //string imgPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "ylderlan.jpg");


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
            ShowMensagem(imgPath, TipoMensagem.IMAGEM, Remetente.SERVIDOR);
        }

        private void _enviarVideo(string videoPath)
        {
            //string videoPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "batata.mp4");

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
            ShowMensagem("Video Enviado "+videoPath, TipoMensagem.TEXTO, Remetente.SERVIDOR);
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