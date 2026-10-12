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

            var currentMode = dictionaries.FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.Contains("LightTheme.xaml") || d.Source.OriginalString.Contains("DarkTheme.xaml")));
            if (currentMode != null)
            {
                dictionaries.Remove(currentMode);
            }

            dictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(themeUri, UriKind.Relative)
            });
        }
    }
}