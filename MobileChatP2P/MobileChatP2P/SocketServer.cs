using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using System.IO;
using Java.Nio;
using Java.IO;

namespace MobileChatP2P
{
    class SocketServer
    {

        IPHostEntry host;
        IPAddress ipAddress;
        IPEndPoint localEndPoint;
        public delegate void MensagemRecebida(string result);

        public SocketServer()
        {
            host = Dns.GetHostEntry("localhost");
            ipAddress = host.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, 11000);
        }

        public void Listen(MensagemRecebida callback)
        {
            
            try
            {

                // Create a Socket that will use Tcp protocol      
                TcpListener listener = new TcpListener(localEndPoint);
                // A Socket must be associated with an endpoint using the Bind method  
                listener.Start();
                
                Socket handler = listener.AcceptSocket();

                // Incoming data from the client.    
                string data = String.Empty;
                TipoMensagem tipo_atual = TipoMensagem.CODE;
                while (handler.Connected)
                {

                    byte[] buffer = new byte[1000000];
                    int dataReceived = handler.Receive(buffer);
                    

                    if(dataReceived == 0)
                    {
                        break;
                    }
                    

                    if(tipo_atual == TipoMensagem.CODE)
                    {
                        data = Encoding.ASCII.GetString(buffer);
                        if (data.Contains("IMAGEM"))
                        {
                            tipo_atual = TipoMensagem.IMAGEM;
                        }else if (data.Contains("TEXTO"))
                        {
                            tipo_atual = TipoMensagem.TEXTO;
                        }
                        else if (data.Contains("VIDEO"))
                        {
                            tipo_atual = TipoMensagem.VIDEO;
                        }
                        callback(data);
                    }else if(tipo_atual == TipoMensagem.TEXTO)
                    {
                        data = Encoding.ASCII.GetString(buffer);
                        callback(data);
                        tipo_atual = TipoMensagem.CODE;
                    }
                    else if (tipo_atual == TipoMensagem.IMAGEM)
                    {
                        data = "Recebendo uma Imagem";
                        callback(data);
                        //Bitmap img = (Bitmap)Bitmap.FromArray(buffer);
                        string fileName = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "kelson.png");

                        //img.Compress(Bitmap.CompressFormat.Png, 100, new FileStream(fileName, FileMode.Create));
                        System.IO.File.WriteAllBytes(fileName, buffer);

                        tipo_atual = TipoMensagem.CODE;
                    }
                    else if (tipo_atual == TipoMensagem.VIDEO)
                    {
                        data = "Recebendo um Video";
                        callback(data);
                        tipo_atual = TipoMensagem.CODE;
                    }
                    
                    
                    
                }
                handler.Close();
                listener.Stop();
                
                
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
