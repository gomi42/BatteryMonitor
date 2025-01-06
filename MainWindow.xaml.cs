using System.Windows;

namespace BatteryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            InitializeComponent();

            ThemeSettings.SetThemeData("pack://application:,,,/BatteryMonitor;component/",
                                       "DarkModeColors.xaml",
                                       "LightModeColors.xaml",
                                       "Styles.xaml");
            ThemeButton.IsChecked = ThemeSettings.GetWindowsTheme() == WindowsTheme.Dark;
            ThemeSettings.SetTheme(this, ThemeButton.IsChecked == true);
        }

        private void ThemeButtonChecked(object sender, RoutedEventArgs e)
        {
            ThemeSettings.SetTheme(this, true);
        }

        private void ThemeButtonUnchecked(object sender, RoutedEventArgs e)
        {
            ThemeSettings.SetTheme(this, false);
        }
    }
}
