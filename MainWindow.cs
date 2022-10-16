using System;
using System.IO;
using System.Collections.Generic;
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
         ? Today Information Panel
         */

        [UI] private Image ImgToday = null;
        [UI] private Label CityName = null;
        [UI] private Label DateOfToday = null;
        [UI] private Label TemperatureOfToday = null;
        [UI] private Label FeelingOfToday = null;
        [UI] private Label DescWeatherToday = null;
        [UI] private Label Latitude = null;
        [UI] private Label Longitude = null;
        [UI] private Label TempMin = null;
        [UI] private Label TempMax = null;
        [UI] private Label HumidityOfToday = null;
        [UI] private Label WindOfToday = null;

        /* 
        ? DateBox 1
         */
        [UI] private Label DateBox1 = null;
        [UI] private Image ImgWeather1 = null;
        [UI] private Label Description1 = null;
        [UI] private Label Time1 = null;
        [UI] private Label Temperature1 = null;
        [UI] private Label Humidity1 = null;

        /* 
        ? DateBox 2
         */
        [UI] private Label DateBox2 = null;
        [UI] private Image ImgWeather2 = null;
        [UI] private Label Description2 = null;
        [UI] private Label Time2 = null;
        [UI] private Label Temperature2 = null;
        [UI] private Label Humidity2 = null;

        /* 
        ? DateBox 3
         */
        [UI] private Label DateBox3 = null;
        [UI] private Image ImgWeather3 = null;
        [UI] private Label Description3 = null;
        [UI] private Label Time3 = null;
        [UI] private Label Temperature3 = null;
        [UI] private Label Humidity3 = null;

        /* 
        ? DateBox 4
         */
        [UI] private Label DateBox4 = null;
        [UI] private Image ImgWeather4 = null;
        [UI] private Label Description4 = null;
        [UI] private Label Time4 = null;
        [UI] private Label Temperature4 = null;
        [UI] private Label Humidity4 = null;

        /* 
        ? DateBox 5
         */
        [UI] private Label DateBox5 = null;
        [UI] private Image ImgWeather5 = null;
        [UI] private Label Description5 = null;
        [UI] private Label Time5 = null;
        [UI] private Label Temperature5 = null;
        [UI] private Label Humidity5 = null;
        /*
         ? Settings
        */
        [UI] private Window Settings_Window = null;
        [UI] private Label SettingsCity = null;
        [UI] private Button SettingsModifyDefaultCity = null;
        [UI] private Button SettingsButtonChangeCity = null;
        [UI] private Stack SettingsStack = null;
        [UI] private Entry SettingsCityName = null;
        [UI] private ComboBoxText Language = null;
        [UI] private Button ChangeLanguage = null;

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

            init_MainWindow();
            Settings.Clicked += Settings_Show_Cliked;
            Settings_Window.DeleteEvent += SettingshWindow_DeleteEvent;
            init_settings();
            SettingsModifyDefaultCity.Clicked += SettingsChangeDefaultCityClicked;
            SettingsButtonChangeCity.Clicked += SettingsChangeCityName;
            ChangeLanguage.Clicked += SettingsChangelanguage;

        }

        private void init_settings()
        {
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            SettingsCity.Text = options["defaultCity"].ToString();

            JObject lang = JObject.Parse(File.ReadAllText("./language.json"));
            for (int i = 0; i < lang["languages"].ToObject<List<string>>().Count; i++)
            {
                Language.Append(lang["languages"][i].ToString().Split(" ")[0], lang["languages"][i].ToString().Split(" ")[1]);
            }
            Language.SetActiveId(options["lang"].ToString());
        }

        private void SettingsChangeDefaultCityClicked(object sender, EventArgs a)
        {
            SettingsStack.GetChildByName("page1").Show();
            SettingsStack.GetChildByName("page0").Hide();
            SettingsStack.GetChildByName("page2").Hide();
        }

        private async void SettingsChangeCityName(object sender, EventArgs a)
        {
            FetchAPI fetch = new FetchAPI();
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            JObject data = await fetch.GetActualInfos(SettingsCityName.Text, options["lang"].ToString());
            if (int.Parse(data["cod"].ToString()) == 200)
            {
                options["defaultCity"] = SettingsCityName.Text;
                File.WriteAllText("./options.json", options.ToString());
                SettingsStack.GetChildByName("page1").Hide();
                SettingsStack.GetChildByName("page0").Hide();
                SettingsStack.GetChildByName("page2").Show();
                SettingsCity.Text = SettingsCityName.Text.Replace(" ", "");
            }
            else
            {
                ErrorDialog.Title = "City not found";
                ErrorText.Text = "Any city found with this name";
                ErrorButton.Clicked -= QuitApplication_Clicked;
                ErrorButton.Clicked += CloseError_Clicked;
                ErrorDialog.Show();
            }
        }
        private void Settings_Show_Cliked(object sender, EventArgs a)
        {
            Settings_Window.Show();
            SettingsStack.GetChildByName("page1").Hide();
            SettingsStack.GetChildByName("page0").Show();
            SettingsStack.GetChildByName("page2").Hide();
        }

        private void SettingsChangelanguage(object sender, EventArgs a)
        {
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            string value = Language.ActiveId;
            options["lang"] = value;
            if (options["lang"].ToString() == "en")
            {
                options["units"] = "f";
            }
            else
            {
                options["units"] = "c";
            }
            File.WriteAllText("./options.json", options.ToString());
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

        private async void init_MainWindow()
        {
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            FetchAPI api = new FetchAPI();
            JObject actual = await api.GetActualInfos(options["defaultCity"].ToString(), options["lang"].ToString());
            actual = FetchAPI.FormatData(actual, options["units"].ToString());

            JObject forecast = await api.GetForecastInfos(options["defaultCity"].ToString(), options["lang"].ToString());
            List<JObject> fcList = FetchAPI.GetCorrectForecastList(forecast, options["units"].ToString());

            string unit = "K";
            if (options["units"].ToString() == "c")
            {
                unit = "°C";
            }
            else if (options["units"].ToString() == "f")
            {
                unit = "°F";
            }

            //? Actual
            int windSpeed = (int)(actual["wind"]["speed"].ToObject<int>() * 3.6);
            string windDir = Libs.GlobalLib.DegreeToDir(actual["wind"]["deg"].ToObject<int>());
            ImgToday.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{actual["weather"][0]["icon"].ToString()}@4x.png");
            CityName.Text = $"{actual["name"].ToString()}, {actual["sys"]["country"]}";
            DateOfToday.Text = DateOnly.FromDateTime(DateTime.Now).ToString();
            TemperatureOfToday.Text = $"{Math.Round(actual["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            FeelingOfToday.Text = $"Feels: {Math.Round(actual["main"]["feels_like"].ToObject<decimal>(), 2).ToString()}{unit}";
            TempMin.Text = $"Min: {Math.Round(actual["main"]["temp_min"].ToObject<decimal>(), 2).ToString()}{unit}";
            TempMax.Text = $"Max: {Math.Round(actual["main"]["temp_max"].ToObject<decimal>()).ToString()}{unit}";
            DescWeatherToday.Text = actual["weather"][0]["description"].ToString();
            Latitude.Text = $"({actual["coord"]["lat"].ToString()}, ";
            Longitude.Text = $"{actual["coord"]["lon"].ToString()})";
            HumidityOfToday.Text = $"{actual["main"]["humidity"].ToString()}%";
            WindOfToday.Text = $"{windSpeed}km/h {windDir}";

            // ? Forcast
            DateBox1.Text = fcList[0]["dt_txt"].ToString().Split(" ")[0].Replace("-", "/");
            ImgWeather1.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{fcList[0]["weather"][0]["icon"].ToString()}@4x.png");
            Description1.Text = fcList[0]["weather"][0]["description"].ToString();
            Time1.Text = fcList[0]["dt_txt"].ToString().Split(" ")[1];
            Temperature1.Text = $"{Math.Round(fcList[0]["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            Humidity1.Text = $"{fcList[0]["main"]["humidity"].ToString()}%";

            DateBox2.Text = fcList[1]["dt_txt"].ToString().Split(" ")[0].Replace("-", "/");
            ImgWeather2.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{fcList[1]["weather"][0]["icon"].ToString()}@4x.png");
            Description2.Text = fcList[1]["weather"][0]["description"].ToString();
            Time2.Text = fcList[1]["dt_txt"].ToString().Split(" ")[1];
            Temperature2.Text = $"{Math.Round(fcList[1]["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            Humidity2.Text = $"{fcList[1]["main"]["humidity"].ToString()}%";

            DateBox3.Text = fcList[2]["dt_txt"].ToString().Split(" ")[0].Replace("-", "/");
            ImgWeather3.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{fcList[2]["weather"][0]["icon"].ToString()}@4x.png");
            Description3.Text = fcList[2]["weather"][0]["description"].ToString();
            Time3.Text = fcList[2]["dt_txt"].ToString().Split(" ")[1];
            Temperature3.Text = $"{Math.Round(fcList[2]["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            Humidity3.Text = $"{fcList[2]["main"]["humidity"].ToString()}%";

            DateBox4.Text = fcList[3]["dt_txt"].ToString().Split(" ")[0].Replace("-", "/");
            ImgWeather4.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{fcList[3]["weather"][0]["icon"].ToString()}@4x.png");
            Description4.Text = fcList[3]["weather"][0]["description"].ToString();
            Time4.Text = fcList[3]["dt_txt"].ToString().Split(" ")[1];
            Temperature4.Text = $"{Math.Round(fcList[3]["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            Humidity4.Text = $"{fcList[3]["main"]["humidity"].ToString()}%";

            DateBox5.Text = fcList[4]["dt_txt"].ToString().Split(" ")[0].Replace("-", "/");
            ImgWeather5.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{fcList[4]["weather"][0]["icon"].ToString()}@4x.png");
            Description5.Text = fcList[4]["weather"][0]["description"].ToString();
            Time5.Text = fcList[4]["dt_txt"].ToString().Split(" ")[1];
            Temperature5.Text = $"{Math.Round(fcList[4]["main"]["temp"].ToObject<decimal>(), 2).ToString()}{unit}";
            Humidity5.Text = $"{fcList[4]["main"]["humidity"].ToString()}%";
        }

        private void CloseError_Clicked(object sender, EventArgs a)
        {
            ErrorDialog.Hide();
        }
    }
}
