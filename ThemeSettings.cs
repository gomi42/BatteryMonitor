using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace BatteryMonitor
{
    public enum WindowsTheme
    {
        Default = 0,
        Light = 1,
        Dark = 2,
        HighContrast = 3
    }

    internal class ThemeSettings
    {
        // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
        // Copied from dwmapi.h
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        // Copied from dwmapi.h
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref bool pvAttribute,
                                                         uint cbAttribute);

        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";

        static string baseUri = "";
        static string darkModeColors = "";
        static string lightModeColors = "";
        static string allStyles = "";

        public static void SetThemeData(string baseUriP, string darkModeColorsP, string lightModeColorsP, string stylesP)
        {
            baseUri = baseUriP;
            darkModeColors = darkModeColorsP;
            lightModeColors = lightModeColorsP;
            allStyles = stylesP;
        }

        public static WindowsTheme GetWindowsTheme()
        {
            var theme = WindowsTheme.Light;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
                    {
                        object registryValueObject = key?.GetValue(RegistryValueName);

                        if (registryValueObject == null)
                        {
                            return WindowsTheme.Light;
                        }

                        int registryValue = (int)registryValueObject;

                        if (SystemParameters.HighContrast)
                        {
                            theme = WindowsTheme.HighContrast;
                        }

                        theme = registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
                    }
                }
                catch
                {
                }
            }

            return theme;
        }

        public static void SetTheme(Window window, bool darkTheme)
        {
            Uri CreateUri(string file) => new Uri(baseUri + file);

            var dark = Application.Current.Resources.MergedDictionaries.FirstOrDefault(t => t.Source.OriginalString.Contains(darkModeColors));
            var light = Application.Current.Resources.MergedDictionaries.FirstOrDefault(t => t.Source.OriginalString.Contains(lightModeColors));
            var styles = Application.Current.Resources.MergedDictionaries.FirstOrDefault(t => t.Source.OriginalString.Contains(allStyles));

            if (styles != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(styles);
            }

            if (dark != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(dark);
            }

            if (light != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(light);
            }

            if (darkTheme)
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = CreateUri(darkModeColors) });
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = CreateUri(lightModeColors) });
            }

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = CreateUri(allStyles) });

            ThemeSettings.SetTitleMode(window, darkTheme);
        }

        private static void SetTitleMode(Window window, bool dark)
        {
            IntPtr hWnd = new WindowInteropHelper(Window.GetWindow(window)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
            bool darkTitle = dark;
            DwmSetWindowAttribute(hWnd, attribute, ref darkTitle, sizeof(uint));
        }
    }
}
