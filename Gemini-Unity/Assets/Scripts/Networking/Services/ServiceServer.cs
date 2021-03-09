using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gemini.Networking.Services
{
    class ServiceServer
    {
        private Server server;

        private Thread thread;

        public ServiceServer( Server server)
        {
            this.server = server;
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            /*
            thread = new Thread(new ThreadStart(ListenForRequests));
            thread.IsBackground = true;
            thread.Start();
            */
            server.Start();
        }

        public void Shutdown()
        {
            server.ShutdownAsync();
        }

        
       /// <summary>
       /// Starts the GRPC server and listens for incoming requests.
       /// This runs in the ServiceServer local Thread.
       /// </summary>       
        void ListenForRequests()
        {
            try
            {
                server.Start();
            }
            catch (InvalidOperationException e)
            {
                Debug.Log(e.StackTrace);
            }
        }
    }
}
