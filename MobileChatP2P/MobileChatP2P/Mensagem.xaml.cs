using System;
using System.Collections.Generic;
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
                    frame.BackgroundColor = Color.PaleVioletRed;
                }
                else
                {
                    frame.BackgroundColor = Color.PaleGreen;
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
                    conteudo = new Image() { Source = menssagem, HeightRequest=150, Aspect=Aspect.AspectFill};
                    break;
                case TipoMensagem.VIDEO:
                    break;
                default:
                    break;
            }
            return new Mensagem(conteudo, remetente, status);
        }

    }
}