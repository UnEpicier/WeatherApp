using System;
using System.IO;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Newtonsoft.Json.Linq;
using Libs;

namespace Weather_App
{
    class MainWindow : Window
    {
        // ? Error Window
        [UI] private Dialog ErrorDialog = null;
        [UI] private Button ErrorButton = null;

        // ? First Launch Window
        [UI] private Dialog FirstLaunchDialog = null;
        [UI] private Label FirstLaunchLabel = null;
        [UI] private Button FirstLaunchButton = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;

            // ? First Launch Window
            string flState = FirstLaunch();
            if (flState != "")
            {
                if (flState == "firstLaunch")
                {
                    FirstLaunchLabel.Text = @"This is the first launch, components files has been created.
Please retart the application after doing
the setup mentionned in the README.md";
                }
                else if (flState == "missing")
                {
                    FirstLaunchLabel.Text = @"Missing API Key, please re-read README.md and follow the steps
to get an API Key then relaunch the app";
                }

                FirstLaunchButton.Clicked += QuitApplication_Clicked;
                FirstLaunchDialog.Show();
            }

            // ? Errror Window
            // Check client connection first
            if (!GlobalLib.HasConnectivity())
            {
                ErrorButton.Clicked += QuitApplication_Clicked;
                ErrorDialog.Show();
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void QuitApplication_Clicked(object sender, EventArgs a)
        {
            Application.Quit();
        }

        private string FirstLaunch()
        {
            JObject f = JObject.Parse(File.ReadAllText("./config.json"));

            if (Libs.GlobalLib.VerifyFiles() || !File.Exists("./config.json"))
            {
                return "firstLaunch";
            }

            if (
                (File.Exists("./config.json") && f.ContainsKey("API_KEY")) &&
                (string.IsNullOrEmpty(f.GetValue("API_KEY")?.ToString()) || string.IsNullOrWhiteSpace(f.GetValue("API_KEY")?.ToString()))
            )
            {
                return "missing";
            }

            return "";
        }
    }
}
