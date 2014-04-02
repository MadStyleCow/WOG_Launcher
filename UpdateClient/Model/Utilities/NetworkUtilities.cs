using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UpdateClient.Model.Utilities
{
    public static class NetworkUtilities
    {
        public static bool DownloadFile(String pInputUrl, String pOutputPath)
        {
            for (int cycles = 0; cycles < Properties.Settings.Default.RetryAttempts; cycles++)
            {
                try
                {
                    new WebClient().DownloadFile(pInputUrl, pOutputPath);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
            return false;
        }
    }
}
