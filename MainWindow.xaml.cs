using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using BatteryInfo;

namespace BatteryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        WindowsTheme currentTheme;
        int currentBatterieIndex;

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

        private void Button_Click_Test1(object sender, RoutedEventArgs e)
        {
            Test();
            Error.Visibility = Visibility.Visible;
        }

        private void Button_Click_Test2(object sender, RoutedEventArgs e)
        {
        }

        private void Test()
        {
            StringBuilder sb = new StringBuilder();

            Error.Content = sb.ToString();
        }
    }
}
