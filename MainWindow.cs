using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Libs;

namespace Weather_App
{
    class MainWindow : Window
    {
        // ? Error Window
        [UI] private Window ErrorWindow = null;
        [UI] private Button ErrorButton = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;

            // ? Errror Window
            ErrorButton.Clicked += ErrorButton_Clicked;
            // Check client connection first
            if (!GlobalLib.HasConnectivity())
            {
                ErrorWindow.Show();
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void ErrorButton_Clicked(object sender, EventArgs a)
        {
            Application.Quit();
        }
    }
}
