using System.Windows;
using System.Windows.Controls;

namespace BatteryMonitor
{
    internal class PercentValue : Control
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(PercentValue), new PropertyMetadata(""));
    }
}
