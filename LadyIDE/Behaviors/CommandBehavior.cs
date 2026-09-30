using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LadyIDE.WPF.Behaviors
{
    public static class CommandBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(CommandBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element,ICommand? value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand? GetCommand(DependencyObject element)
        {
            return (ICommand?)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(CommandBehavior),
                new PropertyMetadata(null));

        public static void SetCommandParameter(DependencyObject element,object? value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object? GetCommandParameter(DependencyObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                return;

            element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
        }

        private static void Element_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if (sender is not DependencyObject element)
                return;

            ICommand? command = GetCommand(element);
            object? parameter = GetCommandParameter(element);

            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }
        }
    }
}
