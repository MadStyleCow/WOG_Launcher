using System;
using System.Windows;
<<<<<<< HEAD
using System.Windows.Forms;
using Client.Core.Utilities;
using Button = System.Windows.Controls.Button;
=======
using System.Windows.Controls;
using Client.Core.Utilities;
using System.IO;
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15

namespace Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
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
            var dialog = new OpenFileDialog { Filter = @"Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (((Button)pSender).Name)
                {
                    case "eSteamButton":
                        ESteam.Text = dialog.FileName;
                        break;

                    case "eA2Button":
<<<<<<< HEAD
                        EArma2.Text = dialog.FileName;
                        break;

                    case "eA2OAButton":
                        EArma2Oa.Text = dialog.FileName;
=======
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
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15
                        break;

                    case "eA2OABetaButton":
                        EArma2OaBeta.Text = dialog.FileName;
                        break;

                    case "eA3Button":
<<<<<<< HEAD
                        EArma3.Text = dialog.FileName;
=======
                        this.eArma3.Text = dialog.FileName;
                        if (eArma3_Mod.Text.Equals(String.Empty))
                        {
                            this.eArma3_Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void DisplayDirectorySelectBox(object pSender, RoutedEventArgs pE)
        {
            var dialog = new FolderBrowserDialog { ShowNewFolderButton = false };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (((Button) pSender).Name)
                {
                    case "eA2ModButton":
                        EArma2Mod.Text = dialog.SelectedPath;
                        break;

                    case "eA2OAModButton":
                        EArma2OaMod.Text = dialog.SelectedPath;
                        break;

                    case "eA3ModButton":
                        EArma3Mod.Text = dialog.SelectedPath;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the values from the local machine class
            ESteam.Text        = LocalMachine.Instance.SteamPath;
            EArma2.Text        = LocalMachine.Instance.A2Path;
            EArma2Oa.Text      = LocalMachine.Instance.A2OaPath;
            EArma2OaBeta.Text  = LocalMachine.Instance.A2OaBetaPath;
            EArma3.Text        = LocalMachine.Instance.A3Path;

            EArma2Mod.Text    = LocalMachine.Instance.A2AddonPath;
            EArma2OaMod.Text  = LocalMachine.Instance.A2OaAddonPath;
            EArma3Mod.Text    = LocalMachine.Instance.A3AddonPath;
        }

        private void eOK_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            LocalMachine.Instance.SteamPath = ESteam.Text;
            LocalMachine.Instance.A2Path = EArma2.Text;
            LocalMachine.Instance.A2AddonPath = EArma2Mod.Text;
            LocalMachine.Instance.A2OaPath = EArma2Oa.Text;
            LocalMachine.Instance.A2OaBetaPath = EArma2OaBeta.Text;
            LocalMachine.Instance.A2OaAddonPath = EArma2OaMod.Text;
            LocalMachine.Instance.A3Path = EArma3.Text;
            LocalMachine.Instance.A3AddonPath = EArma3Mod.Text;
=======
            LocalMachine.Instance.SteamPath = eSteam.Text;
            LocalMachine.Instance.A2Path = eArma2.Text;
            LocalMachine.Instance.A2OAPath = eArma2OA.Text;
            LocalMachine.Instance.A2OABetaPath = eArma2OABeta.Text;
            LocalMachine.Instance.A3Path = eArma3.Text;

            LocalMachine.Instance.A2AddonPath = eArma2_Mod.Text;
            LocalMachine.Instance.A2OAAddonPath = eArma2OA_Mod.Text;
            LocalMachine.Instance.A3AddonPath = eArma3_Mod.Text;
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15

            LocalMachine.Instance.Save();
            Close();
        }

        private void eCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
