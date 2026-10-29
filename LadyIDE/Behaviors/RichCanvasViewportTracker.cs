using System.Windows;
using RichCanvas;

namespace LadyIDE.WPF.Behaviors
{
    public static class RichCanvasViewportTracker
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(RichCanvasViewportTracker),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.RegisterAttached(
                "ViewportSize",
                typeof(Size),
                typeof(RichCanvasViewportTracker),
                new FrameworkPropertyMetadata(Size.Empty));

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static Size GetViewportSize(DependencyObject element)
        {
            return (Size)element.GetValue(ViewportSizeProperty);
        }

        public static void SetViewportSize(DependencyObject element, Size value)
        {
            element.SetValue(ViewportSizeProperty, value);
        }

        private static void OnIsEnabledChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is not RichItemsControl control) return;

            if ((bool)e.NewValue)
            {
                control.Loaded += Control_Loaded;
                control.SizeChanged += Control_SizeChanged;

                if (control.IsLoaded)
                    UpdateViewportSize(control);

                return;
            }

            control.Loaded -= Control_Loaded;
            control.SizeChanged -= Control_SizeChanged;
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not RichItemsControl control) return;

            UpdateViewportSize(control);
        }

        private static void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is not RichItemsControl control) return;

            UpdateViewportSize(control);
        }

        private static void UpdateViewportSize(RichItemsControl control)
        {
            if (control.ActualWidth <= 0 || control.ActualHeight <= 0) return;

            control.SetCurrentValue(
                ViewportSizeProperty,
                new Size(control.ActualWidth, control.ActualHeight));
        }
    }
}