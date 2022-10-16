using System;
using System.IO;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Libs;
using Newtonsoft.Json.Linq;
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
        /* [UI] private Label DateBox1 = null;
        [UI] private Label ImgWeather1 = null;
        [UI] private Label Description1 = null;
        [UI] private Label Time1 = null;
        [UI] private Label Temperature1 = null;
        [UI] private Label Humidity1 = null; */

        /* 
        ? DateBox 2
         */
        /* [UI] private Label DateBox2 = null;
        [UI] private Label ImgWeather2 = null;
        [UI] private Label Description2 = null;
        [UI] private Label Time2 = null;
        [UI] private Label Temperature2 = null;
        [UI] private Label Humidity2 = null; */

        /* 
        ? DateBox 3
         */
        /* [UI] private Label DateBox3 = null;
        [UI] private Label ImgWeather3 = null;
        [UI] private Label Description3 = null;
        [UI] private Label Time3 = null;
        [UI] private Label Temperature3 = null;
        [UI] private Label Humidity3 = null; */

        /* 
        ? DateBox 4
         */
        /* [UI] private Label DateBox4 = null;
        [UI] private Label ImgWeather4 = null;
        [UI] private Label Description4 = null;
        [UI] private Label Time4 = null;
        [UI] private Label Temperature4 = null;
        [UI] private Label Humidity4 = null; */

        /* 
        ? DateBox 5
         */
        /* [UI] private Label DateBox5 = null;
        [UI] private Label ImgWeather5 = null;
        [UI] private Label Description5 = null;
        [UI] private Label Time5 = null;
        [UI] private Label Temperature5 = null;
        [UI] private Label Humidity5 = null; */

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
            JObject actual = await api.GetActualInfos(options["defaultCity"].ToString(), "", options["lang"].ToString());
            actual = FetchAPI.FormatData(actual, options["units"].ToString());

            JObject forecast = await api.GetForecastInfos(options["defaultCity"].ToString(), "", options["lang"].ToString());
            forecast = FetchAPI.FormatForecast(forecast);

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
            DateOfToday.Text = DateTime.Now.Date.ToString();
            TemperatureOfToday.Text = $"{actual["main"]["temp"].ToString()}{unit}";
            FeelingOfToday.Text = $"{actual["main"]["feels_like"].ToString()}{unit}";
            TempMin.Text = $"{actual["main"]["temp_min"].ToString()}{unit}";
            TempMax.Text = $"{actual["main"]["temp_max"].ToString()}{unit}";
            DescWeatherToday.Text = actual["weather"][0]["description"].ToString();
            Latitude.Text = actual["coord"]["lat"].ToString();
            Longitude.Text = actual["coord"]["lon"].ToString();
            HumidityOfToday.Text = $"{actual["main"]["humidity"].ToString()}%";
            WindOfToday.Text = $"{windSpeed}km/h {windDir}";

            /* DateBox1.Text = forecast["list"]["dt_text"].ToString();
            ImgWeather1.Text = forecast["weather"][0]["icon"].ToString();
            Description1.Text = forecast["weather"][0]["description"].ToString();
            Time1.Text = forecast["list"]["dt_text"].ToString();
            Temperature1.Text = forecast["list"]["main"]["temp"].ToString();
            Humidity1.Text = forecast["list"]["main"]["humidity"].ToString(); */
        }
    }
}
