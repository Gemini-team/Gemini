using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Networking.Services.SensorManager
{

    public enum SensorType
    {
        Optical,
        Infrared, 
        Radar,
        Lidar
    } 
    

    class Sensor
    {

        private SensorType type;
        private Int32 sensorWidth;
        private Int32 sensorHeight;
        private string ipAddress;

    }
}
