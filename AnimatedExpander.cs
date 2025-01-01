using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BatteryMonitor
{
    internal class AnimatedExpander : Expander
    {
        FrameworkElement expandContainer;
        FrameworkElement expandSite;
        TranslateTransform transform;
        Duration animationTime = new Duration(TimeSpan.FromMilliseconds(400));

        public AnimatedExpander()
        {
            Expanded += ExpanderHasExpanded;
            Collapsed += ExpanderHasCollapsed;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            expandContainer = GetTemplateChild("Part_ExpandContainer") as FrameworkElement;
            expandSite = GetTemplateChild("Part_ExpandSite") as FrameworkElement;
            transform = GetTemplateChild("Part_Transform") as TranslateTransform;
        }

        private void ExpanderHasExpanded(object sender, RoutedEventArgs args)
        {
            // we can't animate the height of expandContainer, the height isn't known yet
            // even the inner childreen aren't calculated

            expandContainer.Height = double.NaN;
            expandSite.Visibility = Visibility.Visible;

            var animation = new DoubleAnimation(-20, 0, animationTime, FillBehavior.Stop);
            animation.EasingFunction = new CubicEase();
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            animation = new DoubleAnimation(0, 1, animationTime, FillBehavior.Stop);
            animation.EasingFunction = new CubicEase();
            expandSite.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }

        private void ExpanderHasCollapsed(object sender, RoutedEventArgs args)
        {
            var animation = new DoubleAnimation(0, -20, animationTime, FillBehavior.Stop);
            var ease = new CubicEase();
            ease.EasingMode = EasingMode.EaseOut;
            animation.EasingFunction = ease;
            animation.Completed += AnimationCollapsedCompleted;
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            animation = new DoubleAnimation(expandContainer.ActualHeight, 0, animationTime, FillBehavior.Stop);
            ease = new CubicEase();
            ease.EasingMode = EasingMode.EaseOut;
            animation.EasingFunction = ease;
            expandContainer.BeginAnimation(HeightProperty, animation);

            animation = new DoubleAnimation(1, 0, animationTime, FillBehavior.Stop);
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
