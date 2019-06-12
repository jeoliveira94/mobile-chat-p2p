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

        TipoMensagem tipo_atual = TipoMensagem.TEXTO;
        int size_esperado = 0;
        int size_recebido = 0;
        byte[] buffer_total = new byte[100000000];

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
                
                while (handler.Connected)
                {

                    byte[] buffer = new byte[1000000];
                    int dataReceived = handler.Receive(buffer);
                    

                    if(dataReceived == 0)
                    {
                        break;
                    }

                    

                    if (size_recebido == 0)
                    {
                        tipo_atual = (TipoMensagem)buffer[0];
                        for (int i = 1; i < 10; i++)
                        {
                            int pot = 9 - i;
                            int factor = (int)Math.Pow(10, pot);
                            size_esperado += buffer[i] * factor;
                        }
                        size_recebido += dataReceived;

                        for(int i = 10; i < dataReceived; i++)
                        {
                            buffer_total[i - 10] = buffer[i];
                        }
                        callback("Recebendo " + size_esperado+" bytes");
                        callback("Recebendo " + tipo_atual.ToString());
                    }
                    else if(size_recebido < size_esperado)
                    {
                        for(int i = 0; i < dataReceived; i++)
                        {
                            buffer_total[size_recebido + i] = buffer[i];
                        }
                        size_recebido += dataReceived;                        
                    }

                    if (size_recebido >= size_esperado)
                    {
                        byte[] dados_bytes = new byte[size_recebido];

                        for (int i = 0; i < dados_bytes.Length; i++)
                        {
                            dados_bytes[i] = buffer_total[i];
                        }
                        if (tipo_atual == TipoMensagem.TEXTO)
                        {
                            data = Encoding.ASCII.GetString(dados_bytes);
                            callback(data);
                        }
                        else if (tipo_atual == TipoMensagem.IMAGEM)
                        {

                            string fileName = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "kelson.png");

                            System.IO.File.WriteAllBytes(fileName, dados_bytes);

                            //tipo_atual = TipoMensagem.CODE;
                        }
                        else if (tipo_atual == TipoMensagem.VIDEO)
                        {
                            data = "Recebendo um Video";
                            callback(data);
                            //tipo_atual = TipoMensagem.CODE;
                        }

                        buffer_total = new byte[100000000];
                        size_esperado = 0;
                        size_recebido = 0;
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
