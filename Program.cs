using System;
using Gtk;

namespace Weather_App
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {

            Application.Init();

            var app = new Application("org.Weather_App.Weather_App", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
