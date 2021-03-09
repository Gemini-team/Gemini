using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Gemini.Networking.UDP
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram,
            ProtocolType.Udp
            );

        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;


        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);

#if UNITY_EDITOR
                Debug.Log("SEND: " + bytes + ", " + text);
#else
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
#endif
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, 
                    ref epFrom, recv, so);
#if UNITY_EDITOR
                Debug.Log("RECV: " + epFrom.ToString() + ": " + bytes + ", " +
                    Encoding.ASCII.GetString(so.buffer, 0, bytes));
#else
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
#endif
                _socket.SendTo(Encoding.ASCII.GetBytes("Received something 14"), epFrom);

            }, state);
        }
    }
}
