using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using SS = System.Net.Sockets;
using System.Threading;

namespace Socket_Engine
{
    public static class SocketLink
    {
        public static bool SendData(string host, int destPort, string data)
        {
            IPAddress dest = Dns.GetHostAddresses(host)[0]; //Get the destination IP Address
            IPEndPoint ePoint = new IPEndPoint(dest, destPort);
            SS.Socket mySocket = new SS.Socket(SS.AddressFamily.InterNetwork, SS.SocketType.Dgram, SS.ProtocolType.Udp); //Create a socket using the same protocols as in the Javascript file (Dgram and Udp)

            //byte[] outBuffer = Encoding.ASCII.GetBytes(data); //Convert the data to a byte array
            //int nbBytes = mySocket.SendTo(outBuffer, ePoint); //Send the data to the socket

            bool done = SendString(mySocket, ePoint, data);

            mySocket.Close(); //Socket use over, time to close it

            return done;
        }


        /*************************************/
        /****  Private methods            ****/
        /*************************************/

        private static bool SendInt(SS.Socket mySocket, IPEndPoint ePoint, Int32 value)
        {
            value = IPAddress.HostToNetworkOrder(value); //Convert long from Host Byte Order to Network Byte Order
            if (!SendAll(mySocket, ePoint, BitConverter.GetBytes(value))) //Try to send int... If int fails to send
                return false; //Return false: int not successfully sent
            return true; //Return true: int successfully sent
        }

        /*************************************/

        private static bool SendString(SS.Socket mySocket, IPEndPoint ePoint, string message)
        {
            Int32 bufferlength = message.Count(); //Find string buffer length
            if (!SendInt(mySocket, ePoint, bufferlength)) //Send length of string buffer, If sending buffer length fails...
                return false; //Return false: Failed to send string buffer length
            return SendAll(mySocket, ePoint, Encoding.ASCII.GetBytes(message)); //Try to send string buffer
        }

        /*************************************/

        private static bool SendAll(SS.Socket mySocket, IPEndPoint ePoint, byte[] data)
        {
            int totalbytes = data.Length;

            int bytessent = 0; //Holds the total bytes sent
            while (bytessent < totalbytes) //While we still have more bytes to send
            {
                try
                {
                    int nbBytes = Math.Min(5000, totalbytes - bytessent);
                    int RetnCheck = mySocket.SendTo(new ArraySegment<byte>(data, bytessent, nbBytes).ToArray(), ePoint); //Try to send remaining bytes
                    bytessent += RetnCheck; //Add to total bytes sent
                }
                catch
                {
                    return false;
                }
            }
            return true; //Success!
        }
    }
}
