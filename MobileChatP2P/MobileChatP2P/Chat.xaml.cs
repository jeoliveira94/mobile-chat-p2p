using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChatP2P
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public string meuIp { get; } = "192.168.0.0.1";
        public ICommand enviarMensagem { get; }
        public Chat()
        {
            InitializeComponent();

            BindingContext = this;

            enviarMensagem = new Command(_enviarMensagem);
            button.Command = enviarMensagem;

        }

        public void AddMensagem(View conteudo, Mensagem.Remetente remetente)
        {
            stack.Children.Add(new Mensagem(conteudo, remetente));
        }

        private void _enviarMensagem()
        {
            string msg = editor.Text;
            Console.WriteLine(msg);
            AddMensagem(new Label() { Text = msg }, Mensagem.Remetente.Servidor);
        }


    }
}