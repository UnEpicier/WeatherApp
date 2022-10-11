using System;
using System.IO;
using Gtk;
using Newtonsoft.Json.Linq;

namespace Weather_App
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            VerifyFiles();

            Application.Init();

            var app = new Application("org.Weather_App.Weather_App", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }

        private static void VerifyFiles()
        {
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
                File.WriteAllText(@"./config.txt", JObject.Parse(def).ToString());
            }
        }
    }
}
