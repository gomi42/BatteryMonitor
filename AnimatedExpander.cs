using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace BatteryMonitor
{
    internal class AnimatedExpander : Expander
    {
        public AnimatedExpander()
        {
            Expanded += ExpanderHasExpanded;
            Collapsed += ExpanderHasCollapsed;
        }

        FrameworkElement expandSite;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            expandSite = GetTemplateChild("ExpandSite") as FrameworkElement;
        }

        private void ExpanderHasExpanded(object sender, RoutedEventArgs args)
        {
        }

        private void ExpanderHasCollapsed(object sender, RoutedEventArgs args)
        {
        }
    }


}
