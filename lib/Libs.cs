using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;

namespace Libs
{
    class GlobalLib
    {
        public static bool VerifyFiles()
        {
            bool firstLaunch = false;
            if (!File.Exists(@"./options.json"))
            {
                string def = @"{
                    'lang': 'fr',
                    'cities': [],
                    'defaultCity': ''
                }";
                File.WriteAllText(@"./options.json", JObject.Parse(def).ToString());
            }

            if (!File.Exists(@"./config.json"))
            {
                string def = @"{
                    'API_KEY': ''
                }";
                File.WriteAllText(@"./config.json", JObject.Parse(def).ToString());
                firstLaunch = true;
            }

            return firstLaunch;
        }

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