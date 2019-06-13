using System;
using System.Text;

using System.Net;
using System.Net.Sockets;


namespace MobileChatP2P
{
    class SocketServer
    {

        IPHostEntry host;
        IPAddress ipAddress;
        IPEndPoint localEndPoint;
        public delegate void MensagemRecebida(string result, TipoMensagem tipo);

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
                        //callback("Recebendo " + size_esperado+" bytes", TipoMensagem.TEXTO);
                        //callback("Recebendo " + tipo_atual.ToString(), TipoMensagem.TEXTO);
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
                            callback(data, TipoMensagem.TEXTO);
                        }
                        else if (tipo_atual == TipoMensagem.IMAGEM)
                        {
                            string dirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + "MobileChatData";
                            if (!System.IO.Directory.Exists(dirPath))
                            {
                                System.IO.Directory.CreateDirectory(dirPath);
                            }
                            
                            string fileName = System.IO.Path.Combine(dirPath, "img_"+DateTime.Now.ToString("yyyyMMdd_hhmmss")+ ".png");
                   

                            for (int i = 0; i < 5; i++)
                            {
                                Console.WriteLine(fileName);
                            }


                            System.IO.File.WriteAllBytes(fileName, dados_bytes);
                            callback(fileName, TipoMensagem.IMAGEM);
                            //tipo_atual = TipoMensagem.CODE;
                        }
                        else if (tipo_atual == TipoMensagem.VIDEO)
                        {

                            string dirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + "MobileChatData";
                            if (!System.IO.Directory.Exists(dirPath))
                            {
                                System.IO.Directory.CreateDirectory(dirPath);
                            }

                            string fileName = System.IO.Path.Combine(dirPath, "vid_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".mp4");

                            System.IO.File.WriteAllBytes(fileName, dados_bytes);

        
                            data = "Recebendo um Video";
                            //callback(data, TipoMensagem.TEXTO);
                            callback(fileName, TipoMensagem.VIDEO);
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
