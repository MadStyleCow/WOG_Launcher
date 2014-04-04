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
using UpdateClient.Model.Controllers;
using UpdateClient.Model.Enums;
using UpdateClient.Model.Utilities.Classes;
using UpdateClient.UI.Pages;

namespace UpdateClient.UI.Windows
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
        private void eDocumentGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Controller.InitializeController());
        }

        private void eServerSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (eServerSelect.HasItems)
                Controller.ServerSelected(eServerSelect.SelectedItem.ToString());
        }

        private void eSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings SettingsWindow = new Settings();
            SettingsWindow.ShowDialog();
        }

        private void eActionButton_Click(object sender, RoutedEventArgs e)
        {
            Controller.ButtonClicked();
        }

        private void eMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Controller.Shutdown();
        }

        /* UI Access */
        public void SetProgress(Double pProgressValue, String pProgressMessage)
        {
            this.eProgressBar.Value = pProgressValue;
            this.eProgressLabel.Content = pProgressMessage;
        }

        public void SetButtonText(String pButtonText)
        {
            this.eActionButton.Content = pButtonText;
        }

        public void SetServerList(List<String> pValues)
        {
            foreach(String Value in pValues)
            {
                eServerSelect.Items.Add(Value);
            }

            if (eServerSelect.HasItems)
                eServerSelect.SelectedIndex = 0;
        }

        public void SetBrowserTarget(String pUri)
        {
            BrowserPage.SetBrowserTarget(pUri);
        }

        public void SetModFolderList(List<String> pFolderList)
        {
            LauncherPage.SetModFolderList(pFolderList);
        }

        public void SetSettings(SettingsCacheEntry pEntry)
        {
            LauncherPage.SetSettings(pEntry);
        }

        public void GetSettingsRequest()
        {
            Controller.GetSettings(LauncherPage.GetSettings());
        }

        /* Private methods */
        private void LauncherDisplayed()
        {
            Controller.LauncherDisplayed();
        }

        /* Public methods */
        public void SetUIState(AppState pUIState)
        {
            this.UIState = pUIState;

            switch(UIState)
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

        public void SetFrameState(FrameState pFrameState)
        {
            this.UIFrameState = pFrameState;
        }
    }
}
