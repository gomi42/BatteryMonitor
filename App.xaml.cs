using System;
using System.Windows;

namespace BatteryMonitor
{
    public partial class App : Application
    {
        Window window;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            window = new MainWindow();
            window.DataContext = new MainViewModel();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (window.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
