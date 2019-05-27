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
    public partial class Chat : ContentPage
    {
        public string meuIp { get; } = "192.168.0.0.1";
        public Chat()
        {
            InitializeComponent();

            BindingContext = this;
        }
    }
}