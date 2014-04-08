using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UpdateClient.UI.Pages
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Page
    {
        /* Fields */
        System.Windows.Forms.Integration.WindowsFormsHost eHost;
        System.Windows.Forms.WebBrowser eBrowser;

        /* Constructors */
        public Browser()
        {
            // Draw UI
            InitializeComponent();

            // Initialize the WinForm host
            this.eHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            this.eBrowser = new System.Windows.Forms.WebBrowser();
            this.eBrowser.ScrollBarsEnabled = false;
            this.eHost.Child = eBrowser;
            this.eBrowserGrid.Children.Add(this.eHost);
        }

        /* UI Access */
        public void SetBrowserTarget(String pUri)
        {
            eBrowser.Navigate(pUri);
        }
    }
}
