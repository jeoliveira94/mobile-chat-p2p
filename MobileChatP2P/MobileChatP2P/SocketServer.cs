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

        public Task<string> Listen(MensagemRecebida callback)
        {
            
            try
            {

                // Create a Socket that will use Tcp protocol      
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method  
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.  
                // We will listen 10 requests at a time  
                listener.Listen(10);
                
                Socket handler = listener.Accept();

                // Incoming data from the client.    
                string data = null;
                byte[] bytes = null;

                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        //callback(data);
                        Console.WriteLine(data);
                        data = "";
                     //   break;
                    }
                }

                Console.WriteLine("Text received : {0}", data);

                //byte[] msg = Encoding.ASCII.GetBytes(data);
                //handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                return Task.FromResult(data);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Task.FromResult(String.Empty);


        }
    }
}
