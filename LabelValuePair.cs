using System.Windows;
using System.Windows.Controls;

namespace BatteryMonitor
{
    internal class LabelValuePair : ValueControl
    {
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabelValuePair), new PropertyMetadata("??"));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(LabelValuePair), new PropertyMetadata(string.Empty));
    }
}
