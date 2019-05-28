using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MobileChatP2P
{
    public class SocketClient
    {
        Socket sender;
        public IPAddress myIpAddress;

        public SocketClient()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            this.myIpAddress = host.AddressList[0];

            sender = new Socket(myIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SendMessage(string message) {
            byte[] bytes = new byte[1024];

            //byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

            byte[] msg = Encoding.ASCII.GetBytes(message);

            // Send the data through the socket.    
            int bytesSent = sender.Send(msg);

            // Receive the response from the remote device.    
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
        }

        public void StartClient(IPAddress serverIP)
        {
                                
            IPEndPoint remoteEP = new IPEndPoint(serverIP, 11000);              
                

            // Connect the socket to the remote endpoint. Catch any errors.    
            try
            {
                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        public void StopClient()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
