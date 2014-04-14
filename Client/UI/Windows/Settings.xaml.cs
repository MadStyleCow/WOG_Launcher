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
            InitializeComponent();
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

        /*private void eArmA2OAPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA2OAPath.Text = dialog.FileName;
            }
        }

        private void eArmA3PathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA3Path.Text = dialog.FileName;
            }
        }

        private void eArmA2PathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA2Path.Text = dialog.FileName;
            }
        }

        private void eOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (eArmA2Path.Text != String.Empty)
                Properties.Settings.Default.A2_Path = eArmA2Path.Text;

            if (eArmA2OAPath.Text != String.Empty)
                Properties.Settings.Default.A2OA_Path = eArmA2OAPath.Text;

            if (eArmA3Path.Text != String.Empty)
                Properties.Settings.Default.A3_Path = eArmA3Path.Text;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void eCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            eArmA2Path.Text = Properties.Settings.Default.A2_Path;
            eArmA2OAPath.Text = Properties.Settings.Default.A2OA_Path;
            eArmA3Path.Text = Properties.Settings.Default.A3_Path;
        }*/
    }
}
