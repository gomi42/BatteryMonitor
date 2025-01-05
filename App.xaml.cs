using System.Windows;

namespace BatteryMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow();
            window.DataContext = new MainViewModel();
            window.Show();
            MainWindow = window;
        }
    }
}
