using System;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using WebBrowser = System.Windows.Forms.WebBrowser;

namespace Client.UI.Pages
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser
    {
        /* Fields */
        readonly WebBrowser _eBrowser;

        /* Constructors */
        public Browser()
        {
            // Draw UI
            InitializeComponent();

            // Initialize the WinForm host
            var eHost = new WindowsFormsHost();
            _eBrowser = new WebBrowser
            {
                ScrollBarsEnabled = false,
            };
            eHost.Child = _eBrowser;
            EBrowserGrid.Children.Add(eHost);
        }

        /* UI Access */
        public void SetBrowserTarget(String pUri)
        {
            Task.Run(() => _eBrowser.Navigate(pUri));
            //_eBrowser.Navigate(pUri);
        }
    }
}
