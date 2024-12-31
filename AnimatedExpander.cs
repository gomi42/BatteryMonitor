using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
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

        FrameworkElement expandContainer;
        FrameworkElement expandSite;
        TranslateTransform transform;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            expandContainer = GetTemplateChild("Part_ExpandContainer") as FrameworkElement;
            expandSite = GetTemplateChild("Part_ExpandSite") as FrameworkElement;
            transform = GetTemplateChild("Part_Transform") as TranslateTransform;
        }

        private Duration aniTime = new Duration(TimeSpan.FromMilliseconds(400));

        private void ExpanderHasExpanded(object sender, RoutedEventArgs args)
        {
            expandContainer.Height = double.NaN;
            expandSite.Visibility = Visibility.Visible;

            var animation = new DoubleAnimation(-20, 0, aniTime, FillBehavior.Stop);
            animation.EasingFunction = new CubicEase();
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            animation = new DoubleAnimation(0.0, 1, aniTime, FillBehavior.Stop);
            animation.EasingFunction = new CubicEase();
            expandSite.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }

        private void ExpanderHasCollapsed(object sender, RoutedEventArgs args)
        {
            var animation = new DoubleAnimation(0, -20, aniTime, FillBehavior.Stop);
            var ease = new CubicEase();
            ease.EasingMode = EasingMode.EaseOut;
            animation.EasingFunction = ease;
            animation.Completed += AnimationCollapsedCompleted;
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            animation = new DoubleAnimation(expandContainer.ActualHeight, 0, aniTime, FillBehavior.Stop);
            ease = new CubicEase();
            ease.EasingMode = EasingMode.EaseOut;
            animation.EasingFunction = ease;
            expandContainer.BeginAnimation(HeightProperty, animation);

            animation = new DoubleAnimation(1, 1, aniTime, FillBehavior.Stop);
            animation.EasingFunction = new CubicEase();
            expandSite.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }

        private void AnimationCollapsedCompleted(object sender, EventArgs e)
        {
            expandSite.Visibility = Visibility.Collapsed;
            expandContainer.Height = double.NaN;
        }
    }


}
