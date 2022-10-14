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
         ? Search Window
         */
        [UI] private Window SearchWindow = null;
        [UI] private Entry SearchEntry = null;
        [UI] private Button SearchButton = null;
        [UI] private Stack SearchStack = null;
        // Data
        [UI] private Label SearchCity = null;
        [UI] private Label SearchCoords = null;
        [UI] private Label SearchTime = null;
        [UI] private Image SearchImage = null;
        [UI] private Label SearchDescription = null;
        [UI] private Label SearchTemperature = null;
        [UI] private Label SearchFeeling = null;
        [UI] private Label SearchMin = null;
        [UI] private Label SearchMax = null;
        [UI] private Label SearchHumidity = null;
        [UI] private Label SearchWind = null;
        [UI] private Label SearchPressure = null;
        [UI] private Label SearchSunrise = null;
        [UI] private Label SearchSunset = null;
        [UI] private Button SearchAdd = null;

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

            JObject options = JObject.Parse(File.ReadAllText("options.json"));

            /*
             ? Search Window
             */
            Search.Clicked += Search_Clicked;
            SearchButton.Clicked += SearchButton_Clicked;
            SearchAdd.Clicked += SearchAdd_Clicked;
            SearchWindow.DeleteEvent += SearchWindow_DeleteEvent;
            if (options["defaultCity"].ToString() == "")
            {
                SearchWindow.Deletable = false;
                SearchWindow.Show();
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

        private void CloseError_Clicked(object sender, EventArgs a)
        {
            ErrorDialog.Hide();
        }

        /*
         ? Search Window
         */
        private void Search_Clicked(object sender, EventArgs a)
        {
            SearchWindow.Deletable = true;
            SearchWindow.Show();
        }

        private void SearchWindow_DeleteEvent(object sender, DeleteEventArgs a)
        {
            SearchWindow.Hide();
            a.RetVal = true;
        }

        private async void SearchButton_Clicked(object sender, EventArgs a)
        {
            if (SearchEntry.Text != null || SearchEntry.Text != "")
            {
                // Get client options
                JObject options = JObject.Parse(File.ReadAllText("./options.json"));

                SearchStack.GetChildByName("page0").Hide();
                SearchStack.GetChildByName("page1").Show();
                SearchStack.GetChildByName("page2").Hide();

                FetchAPI fetch = new FetchAPI();
                JObject data = await fetch.GetActualInfos(SearchEntry.Text, "", options["lang"].ToString());

                if (int.Parse(data["cod"].ToString()) == 200)
                {
                    string unit = "K";
                    if (options["units"].ToString() == "c")
                    {
                        unit = "°C";
                    }
                    else if (options["units"].ToString() == "f")
                    {
                        unit = "°F";
                    }

                    int windSpeed = (int)(data["wind"]["speed"].ToObject<int>() * 3.6);
                    string windDir = Libs.GlobalLib.DegreeToDir(data["wind"]["deg"].ToObject<int>());
                    string time = Libs.GlobalLib.UnixTimeStampToDateTime(data["dt"].ToObject<double>() + data["timezone"].ToObject<double>()).TimeOfDay.ToString();
                    string utc = $"{Math.Abs(data["timezone"].ToObject<int>() / 3600)}";
                    if (int.Parse(utc) < 10) utc = "0" + utc + ":00";
                    if (data["timezone"].ToObject<int>() == 0)
                    {
                        utc = "";
                    }
                    else if (data["timezone"].ToObject<int>() > 0)
                    {
                        utc = "+" + utc;
                    }
                    else if (data["timezone"].ToObject<int>() < 0)
                    {
                        utc = "-" + utc;
                    }
                    string sunrise = Libs.GlobalLib.UnixTimeStampToDateTime(data["sys"]["sunrise"].ToObject<double>() + data["timezone"].ToObject<double>()).TimeOfDay.ToString();
                    string sunset = Libs.GlobalLib.UnixTimeStampToDateTime(data["sys"]["sunset"].ToObject<double>() + data["timezone"].ToObject<double>()).TimeOfDay.ToString();

                    SearchStack.GetChildByName("page0").Hide();
                    SearchStack.GetChildByName("page1").Hide();
                    SearchStack.GetChildByName("page2").Show();

                    data = FetchAPI.FormatData(data, options["units"].ToString());
                    SearchCity.Text = $"{data["name"].ToString()}, {data["sys"]["country"].ToString()}";
                    SearchCoords.Text = $"({data["coord"]["lat"].ToString()}, {data["coord"]["lon"].ToString()})";
                    SearchTime.Text = $"{time} UTC{utc}";
                    SearchImage.Pixbuf = new Gdk.Pixbuf($"./assets/icons/{data["weather"][0]["icon"].ToString()}@4x.png");
                    SearchDescription.Text = $"{data["weather"][0]["description"].ToString()}";
                    SearchTemperature.Text = $"Température : {data["main"]["temp"].ToString()}{unit}";
                    SearchFeeling.Text = $"Ressenti : {data["main"]["feels_like"].ToString()}{unit}";
                    SearchMin.Text = $"{data["main"]["temp_min"].ToString()}{unit}";
                    SearchMax.Text = $"{data["main"]["temp_max"].ToString()}{unit}";
                    SearchHumidity.Text = $"Humidité : {data["main"]["humidity"].ToString()}%";
                    SearchWind.Text = $"{windSpeed}km/h {windDir}";
                    SearchPressure.Text = $"Pression: {data["main"]["pressure"].ToString()}hPa";
                    SearchSunrise.Text = $"{sunrise}";
                    SearchSunset.Text = $"{sunset}";
                }
                else
                {
                    SearchStack.GetChildByName("page0").Show();
                    SearchStack.GetChildByName("page1").Hide();
                    SearchStack.GetChildByName("page2").Hide();

                    SearchEntry.Text = "";

                    ErrorDialog.Title = "City not found";
                    ErrorText.Text = "Any city found with this name";
                    ErrorButton.Clicked -= QuitApplication_Clicked;
                    ErrorButton.Clicked += CloseError_Clicked;
                    ErrorDialog.Show();
                }

            }
        }

        private void SearchAdd_Clicked(object sender, EventArgs a)
        {
            JObject options = JObject.Parse(File.ReadAllText("./options.json"));
            JArray cities = options["cities"].ToObject<JArray>();

            if (options["defaultCity"].ToString() == "" || cities.Count == 0) options["defaultCity"] = SearchCity.Text.Replace(" ", "");

            if (cities.Contains(SearchCity.Text.Replace(" ", "")) == false)
            {
                cities.Add(SearchCity.Text.Replace(" ", ""));
                options["cities"] = cities;
                File.WriteAllText("./options.json", options.ToString());
            }

            SearchWindow.Hide();
        }
    }
}
