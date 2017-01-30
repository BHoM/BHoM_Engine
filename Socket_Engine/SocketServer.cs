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

    public class SocketServer
    {
        // State object for reading client data asynchronously
        public class StateObject
        {
            public SS.Socket handler = null;                    // Client  socket.
            public byte[] buffer = null;                        // Receive buffer.
            public MessageEvent callback = null;                // Callback method
            public int totalBytesRead = 0;                      // Total number of bytes read so far
        }

        public delegate void MessageEvent(string data);

        public SocketServer()
        {
            m_Socket = new SS.Socket(SS.AddressFamily.InterNetwork, SS.SocketType.Dgram, SS.ProtocolType.Udp);
        }

        public bool Listen(int port = 8888)
        {
            if (m_Port == port)
                return true;
            else if (m_Port != 0)
                m_Socket.Disconnect(true);

            try
            {
                m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));
                m_Port = port;

                StateObject state = new StateObject();
                state.callback = MessageReceived;
                state.handler = m_Socket;

                Thread thread = new Thread(() => ReadMessage(state));
                thread.Start();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public MessageEvent MessageReceived;


        /*************************************/
        /****  Private fields & methods   ****/
        /*************************************/

        private int m_Port = 0;
        private SS.Socket m_Socket = null;

        private static void ReadMessage(StateObject state)
        {
            while (true)
            {
                int bufferSize = ReadInt(state);
                state.buffer = new byte[bufferSize];

                state.totalBytesRead = 0;
                string message = ReadString(state);

                if (state.callback != null)
                    state.callback.Invoke(message);

                Thread.Sleep(100);
            }
            
        }

        private static int ReadInt(StateObject state)
        {
            byte[] buffer = new byte[sizeof(Int32)];
            EndPoint remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            state.handler.ReceiveFrom(buffer, buffer.Length, 0, ref remote);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }

        public static string ReadString(StateObject state)
        {
            int messageLength = state.buffer.Length;
            EndPoint remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            while (state.totalBytesRead < messageLength)
            {
                int receivedDataLength = state.handler.ReceiveFrom(state.buffer, state.totalBytesRead, messageLength - state.totalBytesRead, 0, ref remote);
                state.totalBytesRead += receivedDataLength;
            }

            return Encoding.ASCII.GetString(state.buffer, 0, messageLength);
        }
    }
}