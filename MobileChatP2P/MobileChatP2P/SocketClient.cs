using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;
using Android.Graphics;
using System.Linq;

namespace MobileChatP2P
{
    public class SocketClient
    {
        public TcpClient _Client;
        Stream _Stream;
        public IPAddress myIpAddress;

        public SocketClient()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            this.myIpAddress = host.AddressList[0];

            _Client = new TcpClient();

            
        }

        public void SendMessage(string message) {
            if (_Client.Connected)
            {
                //SendCode(TipoMensagem.TEXTO);
                // Create Instance of an Encoder:
                ASCIIEncoding _Asc = new ASCIIEncoding();

                byte[] _Buffer = new byte[1024];

                // Create Buffer to Send Message:
                _Buffer = _Asc.GetBytes(message);
                byte code = (int)TipoMensagem.TEXTO;
                byte[] mensagem_total = new byte[_Buffer.Length + 10];
                mensagem_total[0] = code;
                var result = GetDigits3(mensagem_total.Length);
                int[] digits = result.ToArray();
                for(int i = 0; i < 9; i++)
                {
                    int curdig = (digits.Length - 1) - i;
                    int curmsg = 9 - i;

                    if (curmsg == 0) break;
                    if(curdig >= 0)
                    {
                        mensagem_total[curmsg] = (byte)digits[curdig];
                    }
                    else
                    {
                        mensagem_total[curmsg] = 0;
                    }
                }
                _Buffer.CopyTo(mensagem_total, 10);
                // Write Message to the Stream:
                _Stream.Write(mensagem_total, 0, mensagem_total.Length);
            }
        }
        public void SendImage(byte[] buffer)
        {
            if (_Client.Connected)
            {
                byte code = (int)TipoMensagem.IMAGEM;
                byte[] mensagem_total = new byte[buffer.Length + 10];
                mensagem_total[0] = code;
                var result = GetDigits3(mensagem_total.Length);
                int[] digits = result.ToArray();
                for (int i = 0; i < 9; i++)
                {
                    int curdig = (digits.Length - 1) - i;
                    int curmsg = 9 - i;

                    if (curmsg == 0) break;
                    if (curdig >= 0)
                    {
                        mensagem_total[curmsg] = (byte)digits[curdig];
                    }
                    else
                    {
                        mensagem_total[curmsg] = 0;
                    }
                }
                buffer.CopyTo(mensagem_total, 10);
                // Write Message to the Stream:
                _Stream.Write(mensagem_total, 0, mensagem_total.Length);
            }
        }

        public void SendVideo(byte[] buffer)
        {
            if (_Client.Connected)
            {
                byte code = (int)TipoMensagem.VIDEO;
                byte[] mensagem_total = new byte[buffer.Length + 10];
                mensagem_total[0] = code;
                var result = GetDigits3(mensagem_total.Length);
                int[] digits = result.ToArray();
                for (int i = 0; i < 9; i++)
                {
                    int curdig = (digits.Length - 1) - i;
                    int curmsg = 9 - i;

                    if (curmsg == 0) break;
                    if (curdig >= 0)
                    {
                        mensagem_total[curmsg] = (byte)digits[curdig];
                    }
                    else
                    {
                        mensagem_total[curmsg] = 0;
                    }
                }
                buffer.CopyTo(mensagem_total, 10);
                // Write Message to the Stream:
                _Stream.Write(mensagem_total, 0, mensagem_total.Length);
            }
        }
        /*private void SendCode(TipoMensagem tipo)
        {
            ASCIIEncoding _Asc = new ASCIIEncoding();
            byte[] _Buffer = new byte[1024];
            _Buffer = _Asc.GetBytes(tipo.ToString());
            _Stream.Write(_Buffer, 0, _Buffer.Length);
        }*/

        public void StartClient(IPAddress serverIP)
        {
            try
            {
                _Client.Connect(serverIP, 11000);
                // Create a Stream:                
                _Stream = _Client.GetStream();
            }
            catch (Exception e)
            {

            }

           
        }

        public void StopClient()
        {
            _Stream.Close();
            _Stream.Dispose();
            _Client.Close();
        }
        public static IEnumerable<int> GetDigits3(int source)
        {
            Stack<int> digits = new Stack<int>();
            while (source > 0)
            {
                var digit = source % 10;
                source /= 10;
                digits.Push(digit);
            }

            return digits;
        }
    }
}
