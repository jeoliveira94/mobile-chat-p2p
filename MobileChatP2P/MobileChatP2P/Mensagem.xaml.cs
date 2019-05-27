using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mensagem : ContentView
    {
        public enum Remetente
        {
            Cliente,
            Servidor
        }
        public Mensagem(View conteudo, Remetente remetente)
        {
            InitializeComponent();
            frame.Content = conteudo;
            if(remetente == Remetente.Cliente)
            {
                frame.Margin = new Thickness(2, 2, 35, 2);
            }
        }

    }
}