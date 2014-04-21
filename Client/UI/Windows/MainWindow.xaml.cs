using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Core.Controllers;
using Client.Core.Enums;
using Client.Core.Utilities.Classes;
using Client.UI.Pages;

namespace Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        /* Constructors */
        public MainWindow()
        {
            // Initialize UI
            InitializeComponent();

            // Initialize a controller
            Controller = new AppController(this);
        }

        /* Fields */
        AppController Controller { get; set; }
        AppState UiState {get; set;}
        Launcher LauncherPage { get; set; }
        readonly Browser _browserPage = new Browser();

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
                if (EServerSelect.HasItems)
                {
                    Controller.ServerSelected(EServerSelect.SelectedItem.ToString());
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
                var settingsWindow = new Settings { Owner = this };
                settingsWindow.ShowDialog();
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

        private void eMainWindow_Closing(object sender, CancelEventArgs e)
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
                EProgressBar.Value = pProgressValue;
                EProgressLabel.Content = pProgressMessage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetServerList(IEnumerable<string> pValues)
        {
            try
            {
                foreach (var value in pValues)
                {
                    EServerSelect.Items.Add(value);
                }

                if (EServerSelect.HasItems)
                {
                    EServerSelect.SelectedIndex = 0;
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
                _browserPage.SetBrowserTarget(pUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetModFolderList(IEnumerable<string> pFolderList)
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
        public void SetUiState(AppState pUiState)
        {
            try
            {
                UiState = pUiState;

                switch (UiState)
                {
                    case AppState.Init:
                        ESettingsButton.IsEnabled = true;
                        ESettingsButton.Visibility = Visibility.Hidden;
                        EServerSelect.IsEnabled = false;
                        EActionButton.IsEnabled = false;
                        EActionButton.Content = "Ponies!";
                        EProgressBar.Value = 0;
                        EProgressLabel.Content = "Checking the application for updates...";
                        break;

                    case AppState.Check:
                        ESettingsButton.IsEnabled = true;
                        ESettingsButton.Visibility = Visibility.Visible;
                        EServerSelect.IsEnabled = true;
                        EActionButton.IsEnabled = true;
                        EActionButton.Content = "Check";
                        EProgressBar.Value = 0;
                        EProgressLabel.Content = "Addon check is required";
                        EMainFrame.Navigate(_browserPage);
                        break;

                    case AppState.Update:
                        ESettingsButton.IsEnabled = true;
                        ESettingsButton.Visibility = Visibility.Visible;
                        EServerSelect.IsEnabled = true;
                        EActionButton.IsEnabled = true;
                        EActionButton.Content = "Update";
                        EProgressBar.Value = 0;
                        EProgressLabel.Content = "An update is required in order to play on this server";
                        break;

                    case AppState.Play:
                        ESettingsButton.IsEnabled = true;
                        ESettingsButton.Visibility = Visibility.Visible;
                        EServerSelect.IsEnabled = true;
                        EActionButton.IsEnabled = true;
                        EActionButton.Content = "Play";
                        EProgressBar.Value = 0;
                        EProgressLabel.Content = "Addon set up-to-date";
                        LauncherPage = new Launcher();
                        EMainFrame.Navigate(LauncherPage);
                        LauncherDisplayed();
                        break;

                    case AppState.Cancelcheck:
                        ESettingsButton.IsEnabled = false;
                        ESettingsButton.Visibility = Visibility.Hidden;
                        EServerSelect.IsEnabled = false;
                        EActionButton.IsEnabled = true;
                        EProgressLabel.Content = "Preparing to check addons...";
                        EActionButton.Content = "Cancel";
                        break;

                    case AppState.Cancelupdate:
                        ESettingsButton.IsEnabled = false;
                        ESettingsButton.Visibility = Visibility.Hidden;
                        EServerSelect.IsEnabled = false;
                        EActionButton.IsEnabled = true;
                        EProgressLabel.Content = "Preparing to update addons...";
                        EActionButton.Content = "Cancel";
                        break;

                    case AppState.Close:
                        ESettingsButton.IsEnabled = false;
                        ESettingsButton.Visibility = Visibility.Hidden;
                        EServerSelect.IsEnabled = false;
                        EActionButton.IsEnabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
