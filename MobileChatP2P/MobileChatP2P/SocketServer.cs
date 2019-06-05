using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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
                while (handler.Connected)
                {

                    byte[] buffer = new byte[1024];
                    int dataReceived = handler.Receive(buffer);

                    if(dataReceived == 0)
                    {
                        break;
                    }

                    Console.WriteLine("Text received : {0}", data);

                    data = Encoding.ASCII.GetString(buffer);
                    callback(data);
                    
                }
                handler.Close();
                listener.Stop();
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
