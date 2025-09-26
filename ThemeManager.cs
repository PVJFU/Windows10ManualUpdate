using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace w10mu
{
    public static class ThemeManager
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";

        // Track if we're already monitoring theme changes to avoid multiple registrations
        private static bool _isMonitoringThemeChanges = false;

        public static bool IsSystemDarkTheme()
        {
            try
            {
                // Try to get the system theme preference on Windows 10/11
                var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
                var value = registryKey?.GetValue(RegistryValueName);
                return value is int theme && theme == 0;
            }
            catch
            {
                // Default to light theme if we can't determine the system preference
                return false;
            }
        }

        public static void SetTheme(bool isDarkTheme)
        {
            var resourceDict = Application.Current.Resources;

            // Remove existing theme dictionaries
            var dictsToRemove = resourceDict.MergedDictionaries
                .Where(d => d.Source?.OriginalString?.Contains("ThemeResources") == true)
                .ToList();

            foreach (var dict in dictsToRemove)
            {
                resourceDict.MergedDictionaries.Remove(dict);
            }

            // Add the appropriate theme dictionary
            var themeUri = isDarkTheme ?
                new Uri("DarkThemeResources.xaml", UriKind.Relative) :
                new Uri("ThemeResources.xaml", UriKind.Relative);

            resourceDict.MergedDictionaries.Add(new ResourceDictionary() { Source = themeUri });
        }

        public static void ApplySystemTheme()
        {
            try
            {
                var isDark = IsSystemDarkTheme();
                SetTheme(isDark);
                
                // Start monitoring theme changes if not already doing so
                if (!_isMonitoringThemeChanges)
                {
                    StartMonitoringThemeChanges();
                }
            }
            catch
            {
                // If theme application fails, continue with default system theme
            }
        }

        private static void StartMonitoringThemeChanges()
        {
            try
            {
                // Monitor registry changes to detect theme switch
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                _isMonitoringThemeChanges = true;
            }
            catch
            {
                // If we can't monitor theme changes, continue without monitoring
            }
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            // Update theme when system theme changes
            if (Application.Current?.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    ApplySystemTheme();
                });
            }
        }
    }
}