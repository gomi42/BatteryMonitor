using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace BatteryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowsTheme currentTheme;

        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            InitializeComponent();

            ThemeSettings.SetThemeData("pack://application:,,,/BatteryMonitor;component/",
                                       "DarkModeColors.xaml",
                                       "LightModeColors.xaml",
                                       "Styles.xaml");
            SetTheme(ThemeSettings.GetWindowsTheme());
        }

        //////////////////////////////////////////////

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            SetTheme(currentTheme == WindowsTheme.Light ? WindowsTheme.Dark : WindowsTheme.Light);
        }

        private void SetTheme(WindowsTheme theme)
        {
            currentTheme = theme;
            var dark = theme == WindowsTheme.Dark;

            if (dark)
            {
                Sonne.Visibility = Visibility.Visible;
                Mond.Visibility = Visibility.Collapsed;
            }
            else
            {
                Sonne.Visibility = Visibility.Collapsed;
                Mond.Visibility = Visibility.Visible;
            }

            ThemeSettings.SetTheme(this, dark);
        }
    }
}
