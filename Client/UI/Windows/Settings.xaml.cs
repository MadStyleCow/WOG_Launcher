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
using System.Windows.Shapes;
using Client.Core.Utilities;

namespace Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        /* Constructors */
        public Settings()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* UI Element Event Handlers */
        private void DisplayFileSelectBox(object pSender, RoutedEventArgs pE)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (((Button)pSender).Name)
                {
                    case "eSteamButton":
                        this.eSteam.Text = dialog.FileName;
                        break;

                    case "eA2Button":
                        this.eArma2.Text = dialog.FileName;
                        break;

                    case "eA2OAButton":
                        this.eArma2OA.Text = dialog.FileName;
                        break;

                    case "eA2OABetaButton":
                        this.eArma2OABeta.Text = dialog.FileName;
                        break;

                    case "eA3Button":
                        this.eArma3.Text = dialog.FileName;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void DisplayDirectorySelectBox(object pSender, RoutedEventArgs pE)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() { ShowNewFolderButton = false };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (((Button) pSender).Name)
                {
                    case "eA2ModButton":
                        this.eArma2_Mod.Text = dialog.SelectedPath;
                        break;

                    case "eA2OAModButton":
                        this.eArma2OA_Mod.Text = dialog.SelectedPath;
                        break;

                    case "eA3ModButton":
                        this.eArma3_Mod.Text = dialog.SelectedPath;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load the values from the local machine class
                this.eSteam.Text        = LocalMachine.Instance.SteamPath;
                this.eArma2.Text        = LocalMachine.Instance.A2Path;
                this.eArma2OA.Text      = LocalMachine.Instance.A2OAPath;
                this.eArma2OABeta.Text  = LocalMachine.Instance.A2OABetaPath;
                this.eArma3.Text        = LocalMachine.Instance.A3Path;

                this.eArma2_Mod.Text    = LocalMachine.Instance.A2AddonPath;
                this.eArma2OA_Mod.Text  = LocalMachine.Instance.A2OAAddonPath;
                this.eArma3_Mod.Text    = LocalMachine.Instance.A3AddonPath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void eOK_Click(object sender, RoutedEventArgs e)
        {
            LocalMachine.Instance.Save();
            this.Close();
        }

        private void eCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
