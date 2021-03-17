
namespace Gemini.Networking.Services
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
