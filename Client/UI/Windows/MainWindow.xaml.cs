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
using Client.Core.Controllers;
using Client.Core.Enums;
using Client.Core.Utilities.Classes;
using Client.UI.Pages;
using log4net;
using log4net.Config;

namespace Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Constructors */
        public MainWindow()
        {
            // Initialize UI
            InitializeComponent();

            // Initialize a controller
            this.Controller = new AppController(this);
        }

        /* Fields */
        AppController Controller { get; set; }
        AppState UIState {get; set;}
        FrameState UIFrameState { get; set; }
        Browser BrowserPage = new Browser();
        Launcher LauncherPage;

        /* UI Event Handlers */
        private void eMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() => Controller.InitializeController());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void eServerSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (eServerSelect.HasItems)
                {
                    Controller.ServerSelected(eServerSelect.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void eSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings SettingsWindow = new Settings() { Owner = this };
                SettingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void eActionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.ButtonClicked();
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void eMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Controller.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /* UI Access */
        public void SetProgress(Double pProgressValue, String pProgressMessage)
        {
            try
            {
                this.eProgressBar.Value = pProgressValue;
                this.eProgressLabel.Content = pProgressMessage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetButtonText(String pButtonText)
        {
            try
            {
                this.eActionButton.Content = pButtonText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetServerList(List<String> pValues)
        {
            try
            {
                foreach (String Value in pValues)
                {
                    eServerSelect.Items.Add(Value);
                }

                if (eServerSelect.HasItems)
                {
                    eServerSelect.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetBrowserTarget(String pUri)
        {
            try
            {
                BrowserPage.SetBrowserTarget(pUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetModFolderList(List<String> pFolderList)
        {
            try
            {
                LauncherPage.SetModFolderList(pFolderList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetSettings(SettingsCacheEntry pEntry)
        {
            try
            {
                LauncherPage.SetSettings(pEntry);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void GetSettingsRequest()
        {
            try
            {
                Controller.GetSettings(LauncherPage.GetSettings());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /* Private methods */
        private void LauncherDisplayed()
        {
            try
            {
                Controller.LauncherDisplayed();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /* Public methods */
        public void SetUIState(AppState pUIState)
        {
            try
            {
                this.UIState = pUIState;

                switch (UIState)
                {
                    case AppState.INIT:
                        eSettingsButton.IsEnabled = false;
                        eServerSelect.IsEnabled = false;
                        eActionButton.IsEnabled = false;
                        eActionButton.Content = "Ponies!";
                        eProgressBar.Value = 0;
                        eProgressLabel.Content = "Checking the application for updates...";
                        break;

                    case AppState.CHECK:
                        eSettingsButton.IsEnabled = true;
                        eServerSelect.IsEnabled = true;
                        eActionButton.IsEnabled = true;
                        eActionButton.Content = "Check";
                        eProgressBar.Value = 0;
                        eProgressLabel.Content = "Addon check is required";
                        eMainFrame.Navigate(BrowserPage);
                        break;

                    case AppState.UPDATE:
                        eSettingsButton.IsEnabled = true;
                        eServerSelect.IsEnabled = true;
                        eActionButton.IsEnabled = true;
                        eActionButton.Content = "Update";
                        eProgressBar.Value = 0;
                        eProgressLabel.Content = "An update is required in order to play on this server";
                        break;

                    case AppState.PLAY:
                        eSettingsButton.IsEnabled = true;
                        eServerSelect.IsEnabled = true;
                        eActionButton.IsEnabled = true;
                        eActionButton.Content = "Play";
                        eProgressBar.Value = 0;
                        eProgressLabel.Content = "Addon set up-to-date";
                        LauncherPage = new Launcher();
                        eMainFrame.Navigate(LauncherPage);
                        LauncherDisplayed();
                        break;

                    case AppState.CANCELCHECK:
                    case AppState.CANCELUPDATE:
                        eSettingsButton.IsEnabled = false;
                        eServerSelect.IsEnabled = false;
                        eActionButton.IsEnabled = true;
                        eActionButton.Content = "Cancel";
                        break;

                    case AppState.CLOSE:
                        eSettingsButton.IsEnabled = false;
                        eServerSelect.IsEnabled = false;
                        eActionButton.IsEnabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetFrameState(FrameState pFrameState)
        {
            try
            {
                this.UIFrameState = pFrameState;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
