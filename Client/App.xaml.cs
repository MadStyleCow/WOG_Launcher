using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Client.Core.Utilities;
using Client.Core.Classes;
using Client.Core.Utilities.Classes;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Configure the logger
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
