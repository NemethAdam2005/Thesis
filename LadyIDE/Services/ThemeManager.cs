using System;
using System.Windows;

namespace LadyIDE.WPF.Services
{
    public static class ThemeManager
    {
        public static void SetTheme(bool isLightMode)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            var themeUri = isLightMode
                ? "Themes/LightTheme.xaml"
                : "Themes/DarkTheme.xaml";

            dictionaries.Clear();

            dictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(themeUri, UriKind.Relative)
            });
        }
    }
}