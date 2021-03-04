using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Networking.Services
{
    static class ServicePortGenerator
    {
        static int port = 50080;

        public static int GenPort()
        {
            return port++;
        }
    }
}
