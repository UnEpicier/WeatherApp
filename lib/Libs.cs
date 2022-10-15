using System;
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
                    'defaultCity': '',
                    'units': 'c'
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

        public static string FirstLaunch()
        {
            bool fl = VerifyFiles();

            if (fl)
            {
                return "firstLaunch";
            }
            else
            {
                // ? config.json
                JObject f = JObject.Parse(File.ReadAllText("./config.json"));

                if (!f.ContainsKey("API_KEY"))
                {
                    File.Delete("./config.json");
                    VerifyFiles();
                    return "missing";
                }
                else
                {

                    if (string.IsNullOrEmpty(f.GetValue("API_KEY")?.ToString()) || string.IsNullOrWhiteSpace(f.GetValue("API_KEY")?.ToString()))
                    {
                        return "missing";
                    }
                }

                // ? options.json
                JObject c = JObject.Parse(File.ReadAllText("./options.json"));
                if (!c.ContainsKey("lang") || !c.ContainsKey("defaultCity") || !c.ContainsKey("units"))
                {
                    File.Delete("./options.json");
                    VerifyFiles();
                    return "options";
                }
            }

            return "";
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

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }

        public static string DegreeToDir(int degree)
        {
            if (degree > 22.5 && degree <= 67.5)
            {
                return "NE";
            }
            else if (degree > 67.5 && degree <= 112.5)
            {
                return "E";
            }
            else if (degree > 112.5 && degree <= 157.5)
            {
                return "SE";
            }
            else if (degree > 157.5 && degree <= 202.5)
            {
                return "S";
            }
            else if (degree > 202.5 && degree <= 247.5)
            {
                return "SO";
            }
            else if (degree > 247.5 && degree <= 292.5)
            {
                return "O";
            }
            else if (degree > 292.5 && degree <= 337.5)
            {
                return "NO";
            }
            return "";
        }
    }
}