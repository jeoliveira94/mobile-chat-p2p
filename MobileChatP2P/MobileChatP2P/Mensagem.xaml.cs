using Android.Graphics;
using Android.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChatP2P
{
    public enum TipoMensagem
    {
        TEXTO = 0,
        IMAGEM = 1,
        VIDEO = 2,
    }

    public enum Remetente
    {
        CLIENTE,
        SERVIDOR
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mensagem : ContentView
    {
        
        private Mensagem(View conteudo, Remetente remetente, int status)
        {
            InitializeComponent();
            frame.Content = conteudo;
            if(remetente == Remetente.CLIENTE)
            {
                frame.Margin = new Thickness(2, 2, 35, 2);
                if (status == 0)
                {
                    frame.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
                }
                else
                {
                    frame.BackgroundColor = Xamarin.Forms.Color.PaleGreen;
                }
            }
        }

        public static Mensagem CriarMensagem(string menssagem, TipoMensagem tipo, Remetente remetente, int status)
        {
            
            
            View conteudo = null;
            switch (tipo)
            {   
                case TipoMensagem.TEXTO:
                    conteudo = new Label() { Text = menssagem };
                    break;
                case TipoMensagem.IMAGEM:
                    conteudo = new Xamarin.Forms.Image() { Source = menssagem, HeightRequest=150, Aspect=Aspect.AspectFill};
                    break;
                case TipoMensagem.VIDEO:
                    conteudo = new Label() { Text = "Video Recebido: "+menssagem };
                    
                    /*var img = new Xamarin.Forms.Image();
                    string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + "manifesta.mp4";

                    var existeVideo = System.IO.File.Exists(path);
                    img.Source = GenerateThumbImage(path, 3);
                    img.HeightRequest = 150;
                    img.Aspect = Aspect.AspectFill;
                    conteudo = img;*/
                    
                    break;
                default:
                    break;
            }
            return new Mensagem(conteudo, remetente, status);
        }
        public static ImageSource GenerateThumbImage(string url, long usecond)
        {
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(url, new Dictionary<string, string>());
            Bitmap bitmap = retriever.GetFrameAtTime(usecond);
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return ImageSource.FromStream(() => new MemoryStream(bitmapData));
            }
            return null;
        }
    }
}