using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace w10mu {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ThemeManager.ApplySystemTheme();
            }
            catch (Exception)
            {
                // If theme application fails, continue with default theme
            }
            base.OnStartup(e);
        }
    }
}
