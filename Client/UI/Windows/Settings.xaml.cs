using System;
using System.Windows;
using System.Windows.Forms;
using Client.Core.Utilities;
using System.IO;
using Button = System.Windows.Controls.Button;


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
                        this.EArma2.Text = dialog.FileName;
                        if (EArma2Mod.Text.Equals(String.Empty))
                        {
                            this.EArma2Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
                        break;

                    case "eA2OAButton":
                        this.EArma2Oa.Text = dialog.FileName;
                        if (EArma2OaMod.Text.Equals(String.Empty))
                        {
                            this.EArma2OaMod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
                        break;

                    case "eA2OABetaButton":
                        EArma2OaBeta.Text = dialog.FileName;
                        break;

                    case "eA3Button":
                        this.EArma3.Text = dialog.FileName;
                        if (EArma3Mod.Text.Equals(String.Empty))
                        {
                            this.EArma3Mod.Text = Path.GetDirectoryName(dialog.FileName);
                        }
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
            LocalMachine.Instance.SteamPath = ESteam.Text;
            LocalMachine.Instance.A2Path = EArma2.Text;
            LocalMachine.Instance.A2AddonPath = EArma2Mod.Text;
            LocalMachine.Instance.A2OaPath = EArma2Oa.Text;
            LocalMachine.Instance.A2OaBetaPath = EArma2OaBeta.Text;
            LocalMachine.Instance.A2OaAddonPath = EArma2OaMod.Text;
            LocalMachine.Instance.A3Path = EArma3.Text;
            LocalMachine.Instance.A3AddonPath = EArma3Mod.Text;

            LocalMachine.Instance.Save();
            Close();
        }

        private void eCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
