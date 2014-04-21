using log4net.Config;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            // Configure the logger
            XmlConfigurator.Configure();
        }
    }
}
