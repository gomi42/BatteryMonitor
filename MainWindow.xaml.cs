using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            CollapseAllExpanders(this);
        }

        private void ThemeButtonUnchecked(object sender, RoutedEventArgs e)
        {
            ThemeSettings.SetTheme(this, false);
            CollapseAllExpanders(this);
        }

        public static void CollapseAllExpanders(DependencyObject myVisual)
        {
            if (myVisual is ScrollViewer scrollViewer)
            {
                myVisual = (DependencyObject)scrollViewer.Content;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                var childVisual = VisualTreeHelper.GetChild(myVisual, i);

                if (childVisual is AnimatedExpander expander)
                {
                    expander.IsExpanded = false;
                }

                CollapseAllExpanders(childVisual);
            }
        }
    }
}
