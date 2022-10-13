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
        /*
         ? Custom Error Window
         */
        [UI] private Dialog ErrorDialog = null;
        [UI] private Label ErrorText = null;
        [UI] private Button ErrorButton = null;

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
                    ErrorText.Text = @"This is the first launch, components files has been created.
Please retart the application after doing
the setup mentionned in the README.md";
                }
                else if (flState == "missing")
                {
                    ErrorText.Text = @"Missing API Key, please re-read README.md and follow the steps
to get an API Key then relaunch the app";
                }

                ErrorDialog.Show();
            }

            // ? If any INternet connection
            // Check client connection first
            if (!GlobalLib.HasConnectivity())
            {
                ErrorText.Text = @"Any Internet connection found.
Please check this problem then reopen the application.";
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

    }
}
