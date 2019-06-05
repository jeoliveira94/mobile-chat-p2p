using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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
                // Create Instance of an Encoder:
                ASCIIEncoding _Asc = new ASCIIEncoding();

                byte[] _Buffer = new byte[1024];

                // Create Buffer to Send Message:
                _Buffer = _Asc.GetBytes(message);                

                // Write Message to the Stream:
                _Stream.Write(_Buffer, 0, _Buffer.Length);
            }
        }

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
    }
}
