using System.Net.NetworkInformation;

namespace Libs
{
    class GlobalLib
    {
        public static bool HasConnectivity()
        {
            bool connectionExists = false;
            try
            {
                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                options.DontFragment = true;
                if (!string.IsNullOrEmpty("8.8.8.8"))
                {
                    System.Net.NetworkInformation.PingReply reply = pingSender.Send("8.8.8.8");
                    connectionExists = reply.Status ==
        System.Net.NetworkInformation.IPStatus.Success ? true : false;
                }
            }
            catch (PingException ex)
            {
                return ex != null ? false : false;
            }
            return connectionExists;
        }
    }
}