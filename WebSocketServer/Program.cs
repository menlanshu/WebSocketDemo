using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSocketServer
{
    class SynchronousSocketListener
    {
        public static string data = null;

        public static void StartListening()
        {
            // data buffer for incoming data
            byte[] bytes = new byte[1024];

            // Establish the local endpoint for the socket
            // Dns.GetHostName returns the name of the 
            // host running the application
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = iPHostEntry.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and
            // listen for incoming connections
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections
                while(true)
                {
                    Console.WriteLine("Waiting for a conection...");

                    // Program is suspended while waiting for an incoming connections
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed
                    while(true)
                    {
                        int byteRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, byteRec);
                        if(data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Show the data on the console
                    Console.WriteLine($"Text received :{data}");

                    // Echo the data back to the client
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue..");
            Console.Read();
        }
        static int Main(string[] args)
        {
            StartListening();
            return 0;
        }
    }
}
