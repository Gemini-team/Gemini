using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Networking.Services
{
    class SensorIDGenerator
    {
        static int ID = 1;

        public static int GenID()
        {
            return ID++;
        }
    }
}
