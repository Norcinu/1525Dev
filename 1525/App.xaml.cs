using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		void Application_Startup(object sender, StartupEventArgs e)
		{
			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
											    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(
											    CultureInfo.CurrentCulture.IetfLanguageTag)));
		}
        
        void Application_Exit(object sender, ExitEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Exiting");
#endif
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationWindow app = new ApplicationWindow();
            var context = new PDTUtils.MVVM.ApplicationViewModel();
            app.DataContext = context;
            app.Show();
        }
    }
}

