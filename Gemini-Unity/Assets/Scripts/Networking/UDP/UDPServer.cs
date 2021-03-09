using UnityEngine;

namespace Gemini.Networking.UDP
{
    public class UDPServer : MonoBehaviour
    {
        UDPSocket serverSock;

        // Start is called before the first frame update
        void Start()
        {
            serverSock = new UDPSocket();
            serverSock.Server("127.0.0.1", 50090);
            //serverSock.Send("Received")
        }
    }
}
