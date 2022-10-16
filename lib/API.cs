using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace API
{

    class FetchAPI
    {
        private string API_KEY { get; set; }

        /**
         * {0} => City name
         * {1} => Lang Code (Optional)
         * {2} => API_KEY
         * 
         * Doc: https://openweathermap.org/current#geocoding
         */
        private static string actualURL = "https://api.openweathermap.org/data/2.5/weather?q={0}{1}&appid={2}";
        private static string forecastURL = "https://api.openweathermap.org/data/2.5/forecast?q={0}{1}&appid={2}";

        public FetchAPI()
        {
            // Get API Key from config file and store it
            JObject config = JObject.Parse(File.ReadAllText("./config.json"));
            this.API_KEY = config["API_KEY"].ToString();
        }

        public async Task<JObject> GetActualInfos(string city, string lang = "")
        {
            string l = "";
            if (lang != "")
            {
                l = $"&lang={lang}";
            }

            string formated = string.Format(actualURL, city, l, this.API_KEY);

            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(formated);
            HttpContent content = res.Content;

            string data = await content.ReadAsStringAsync();

            if (data != null)
            {
                return JObject.Parse(data);
            }

            return null;
        }

        public async Task<JObject> GetForecastInfos(string city, string lang = "")
        {
            string l = "";
            if (lang != "")
                l = $"&lang={lang}";
            {
            }

            string formated = string.Format(forecastURL, city, l, this.API_KEY);

            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(formated);
            HttpContent content = res.Content;

            string data = await content.ReadAsStringAsync();

            if (data != null)
            {
                return JObject.Parse(data);
            }

            return null;
        }
        public static JObject FormatData(JObject data, string units)
        {
            bool forecast = false; // False = Actual, True = Forcast
            if (data.ContainsKey("cnt"))
            {
                forecast = true;
            }

            if (units == "c")
            {
                if (!forecast)
                {
                    data["main"]["temp"] = ToCelsius(((int)data["main"]["temp"]));
                    data["main"]["feels_like"] = ToCelsius(((int)data["main"]["feels_like"]));
                    data["main"]["temp_min"] = ToCelsius(((int)data["main"]["temp_min"]));
                    data["main"]["temp_max"] = ToCelsius(((int)data["main"]["temp_max"]));
                }
                else
                {
                    for (int i = 0; i < ((int)data["list"]); i++)
                    {
                        data["list"][i]["temp"] = ToCelsius(((int)data["main"]["temp"])); ;
                        data["list"][i]["feels_like"] = ToCelsius(((int)data["main"]["feels_like"])); ;
                        data["list"][i]["temp_min"] = ToCelsius(((int)data["main"]["temp_min"])); ;
                        data["list"][i]["temp_max"] = ToCelsius(((int)data["main"]["temp_max"])); ;
                    }
                }
            }
            else if (units == "f")
            {
                if (!forecast)
                {
                    data["main"]["temp"] = ToFahrenheit(((int)data["main"]["temp"]));
                    data["main"]["feels_like"] = ToFahrenheit(((int)data["main"]["feels_like"]));
                    data["main"]["temp_min"] = ToFahrenheit(((int)data["main"]["temp_min"]));
                    data["main"]["temp_max"] = ToFahrenheit(((int)data["main"]["temp_max"]));
                }
                else
                {
                    for (int i = 0; i < ((int)data["list"]); i++)
                    {
                        data["list"][i]["temp"] = ToFahrenheit(((int)data["main"]["temp"])); ;
                        data["list"][i]["feels_like"] = ToFahrenheit(((int)data["main"]["feels_like"])); ;
                        data["list"][i]["temp_min"] = ToFahrenheit(((int)data["main"]["temp_min"])); ;
                        data["list"][i]["temp_max"] = ToFahrenheit(((int)data["main"]["temp_max"])); ;
                    }
                }
            }

            return data;
        }

        public static List<JObject> GetCorrectForecastList(JObject data, string unit)
        {
            if (data.ContainsKey("cnt"))
            {
                List<JObject> res = new List<JObject>();
                for (int i = 0; i < data["list"].ToObject<JArray>().Count; i++)
                {
                    if (data["list"][i]["dt_txt"].ToString().Split(" ")[1] == "12:00:00")
                    {
                        res.Add(data["list"][i].ToObject<JObject>());
                    }
                }

                for (int i = 0; i < res.Count; i++)
                {
                    if (unit == "c")
                    {
                        res[i]["main"]["temp"] = ToCelsius((int)(res[i]["main"]["temp"]));
                        res[i]["main"]["feels_like"] = ToCelsius((int)(res[i]["main"]["feels_like"]));
                        res[i]["main"]["temp_min"] = ToCelsius((int)(res[i]["main"]["temp_min"]));
                        res[i]["main"]["temp_max"] = ToCelsius((int)(res[i]["main"]["temp_max"]));
                    }
                    else if (unit == "f")
                        res[i]["main"]["temp"] = ToFahrenheit((int)(res[i]["main"]["temp"]));
                    res[i]["main"]["feels_like"] = ToFahrenheit((int)(res[i]["main"]["feels_like"]));
                    res[i]["main"]["temp_min"] = ToFahrenheit((int)(res[i]["main"]["temp_min"]));
                    res[i]["main"]["temp_max"] = ToFahrenheit((int)(res[i]["main"]["temp_max"]));
                    {

                    }
                }

                return res;
            }

            return new List<JObject>();
        }

        private static double ToCelsius(double kelvin)
        {
            return Math.Round(kelvin - 273.15, 2);
        }

        private static double ToFahrenheit(double kelvin)
        {
            return Math.Round(ToCelsius(kelvin) * (9 / 5) + 32, 2);
        }
    }
}