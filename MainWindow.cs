using System;
using System.IO;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Newtonsoft.Json.Linq;
using Libs;
using API;

namespace Weather_App
{
    class MainWindow : Window
    {
        /*
         ? Custom Error Window
         */
        [UI] private Dialog ErrorDialog = null;
        [UI] private Label ErrorText = null;
        [UI] private Button ErrorButton = null;

        /*
         ? MainWindow
         */
        [UI] private Button Search = null;
        [UI] private Button Settings = null;

        /*
         ? Settings
        */
        [UI] private Window Settings_Window = null;
        [UI] private Label SettingsCity = null;
        [UI] private Button SettingsModifyDefaultCity = null;
        [UI] private Button SettingsButtonChangeCity = null;
        [UI] private Stack SettingsStack = null;
        [UI] private Entry SettingsCityName = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            //? Global
            DeleteEvent += Window_DeleteEvent;
            ErrorButton.Clicked += QuitApplication_Clicked;

            //? If it's the first launch or missing files/api key
            string flState = Libs.GlobalLib.FirstLaunch();
            if (flState != "")
            {
                if (flState == "firstLaunch")
                {
                    ErrorDialog.Title = "First Launch";
                    ErrorText.Text = @"This is the first launch, components files has been created.
Please retart the application after doing
the setup mentionned in the README.md";
                }
                else if (flState == "missing")
                {
                    ErrorDialog.Title = "Missing API Key";
                    ErrorText.Text = @"Missing API Key, please re-read README.md and follow the steps
to get an API Key then relaunch the app";
                }
                else if (flState == "options")
                {
                    ErrorDialog.Title = "Corrupted options file";
                    ErrorText.Text = "Options file corrupted ! Recreating one with default values";
                }

                ErrorDialog.Show();
            }

            // ? If any Internet connection
            if (!GlobalLib.HasConnectivity())
            {
                ErrorDialog.Title = "Any Internet Connection";
                ErrorText.Text = @"Any Internet connection found.
Please check this problem then reopen the application.";
                ErrorDialog.Show();
            }
            Settings.Clicked += Settings_Show_Cliked;
            Settings_Window.DeleteEvent += SettingshWindow_DeleteEvent;
            init_settings();
            SettingsModifyDefaultCity.Clicked += SettingsChangeDefaultCityClicked;
            SettingsButtonChangeCity.Clicked += SettingsChangeCityName;

        }

        private void init_settings() {
            /* JObject lang = JObject.Parse(File.ReadAllText("./language.json"));
            for (int i = 0; i < lang.Count; i++) {
                language.Append(i.ToString(), lang[i].ToString());
            } */
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            SettingsCity.Text = options["defaultCity"].ToString();
        }

        private void SettingsChangeDefaultCityClicked(object sender, EventArgs a) {
             SettingsStack.GetChildByName("page1").Show();
             SettingsStack.GetChildByName("page0").Hide();
             SettingsStack.GetChildByName("page2").Hide();
        }

        private async void  SettingsChangeCityName(object sender, EventArgs a) {
            FetchAPI fetch = new FetchAPI();
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            JObject data = await fetch.GetActualInfos(SettingsCityName.Text, "", options["lang"].ToString());
            if (int.Parse(data["cod"].ToString()) == 200) {
                options["defaultCity"] = SettingsCityName.Text;
                File.WriteAllText("./options.json", options.ToString());
                SettingsStack.GetChildByName("page1").Hide();
                SettingsStack.GetChildByName("page0").Hide();
                SettingsStack.GetChildByName("page2").Show();
                SettingsCity.Text = SettingsCityName.Text.Replace(" ","");
            } else {
                ErrorDialog.Title = "City not found";
                ErrorText.Text = "Any city found with this name";
                ErrorButton.Clicked -= QuitApplication_Clicked;
                ErrorButton.Clicked += CloseError_Clicked;
                ErrorDialog.Show();
            }
        }
        private void Settings_Show_Cliked(object sender, EventArgs a) {
            Settings_Window.Show();
            SettingsStack.GetChildByName("page1").Hide();
             SettingsStack.GetChildByName("page0").Show();
             SettingsStack.GetChildByName("page2").Hide();
        }
        private void SettingshWindow_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Settings_Window.Hide();
            a.RetVal = true;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void QuitApplication_Clicked(object sender, EventArgs a)
        {
            Application.Quit();
        }

        private void CloseError_Clicked(object sender, EventArgs a)
        {
            ErrorDialog.Hide();
        }
    }
}
