using System;
using System.Windows;
using System.Windows.Controls;
using Client.Core.Utilities;
using System.IO;

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
                        if (eArma2_Mod.Text.Equals(String.Empty))
                        {
                            this.eArma2_Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
                        break;

                    case "eA2OAButton":
                        this.eArma2OA.Text = dialog.FileName;
                        if (eArma2OA_Mod.Text.Equals(String.Empty))
                        {
                            this.eArma2OA_Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
                        break;

                    case "eA2OABetaButton":
                        this.eArma2OABeta.Text = dialog.FileName;
                        break;

                    case "eA3Button":
                        this.eArma3.Text = dialog.FileName;
                        if (eArma3_Mod.Text.Equals(String.Empty))
                        {
                            this.eArma3_Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
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
            LocalMachine.Instance.SteamPath = eSteam.Text;
            LocalMachine.Instance.A2Path = eArma2.Text;
            LocalMachine.Instance.A2OAPath = eArma2OA.Text;
            LocalMachine.Instance.A2OABetaPath = eArma2OABeta.Text;
            LocalMachine.Instance.A3Path = eArma3.Text;

            LocalMachine.Instance.A2AddonPath = eArma2_Mod.Text;
            LocalMachine.Instance.A2OAAddonPath = eArma2OA_Mod.Text;
            LocalMachine.Instance.A3AddonPath = eArma3_Mod.Text;

            LocalMachine.Instance.Save();
            this.Close();
        }

        private void eCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
