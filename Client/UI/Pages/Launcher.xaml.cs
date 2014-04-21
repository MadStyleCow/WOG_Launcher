using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Client.Core.Utilities.Classes;

namespace Client.UI.Pages
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher
    {
        /* Fields */
        Guid CurrentServer { get; set; }

        /* Constructors */
        public Launcher()
        {
            InitializeComponent();
        }

        /* UI Access */
        public void SetModFolderList(IEnumerable<string> pFolderList)
        {
            foreach(var folder in pFolderList)
            {
                EMods.Items.Add(new CheckBox
                {
                        Content = new TextBlock
                        {
                             Text = folder
                        }
                    });
            }
        }

        public void SetSettings(SettingsCacheEntry pEntry)
        {
            CurrentServer = pEntry.ServerIdKey;

            foreach (var mod in pEntry.ModList)
            {
                foreach (CheckBox item in EMods.Items)
                {
                    if (((TextBlock)item.Content).Text.ToLowerInvariant().Equals(mod.ToLowerInvariant()))
                    {
                        item.IsChecked = true;
                    }
                }
            }

            ESkipIntro.IsChecked = pEntry.NoSplash;
            EShowScriptErrors.IsChecked = pEntry.ShowScriptErrors;
            EWinXp.IsChecked = pEntry.WinXp;
            EWindowMode.IsChecked = pEntry.Windowed;
            EEmptyWorld.IsChecked = pEntry.EmptyWorld;
            ECpuCountCheck.IsChecked = pEntry.CpuCountChecked;
            if (ECpuCountCheck.IsChecked == true)
                ECpuCount.Text = pEntry.CpuCount.ToString(CultureInfo.InvariantCulture);
            EMaxMemoryCheck.IsChecked = pEntry.MaxMemoryChecked;
            if (EMaxMemoryCheck.IsChecked == true)
                EMaxMemory.Text = pEntry.MaxMemory.ToString(CultureInfo.InvariantCulture);
            EExThreadsCheck.IsChecked = pEntry.ExThreadsChecked;
            if (EExThreadsCheck.IsChecked == true)
                EExThreads.Text = pEntry.ExThreads.ToString(CultureInfo.InvariantCulture);
            EAdditionalParameters.Text = pEntry.AdditionalParameters;
            EConnect.IsChecked = pEntry.AutoConnect;
        }

        public SettingsCacheEntry GetSettings()
        {
            try
            {
                // Get list of selected mods
                var modList = (from CheckBox item in EMods.Items where item.IsChecked == true select ((TextBlock) item.Content).Text).ToList();

                return new SettingsCacheEntry
                {
                    ServerIdKey = CurrentServer,
                    AutoConnect = (EConnect.IsChecked == true),
                    Windowed = (EWindowMode.IsChecked == true),
                    ShowScriptErrors = (EShowScriptErrors.IsChecked == true),
                    WinXp = (EWinXp.IsChecked == true),
                    EmptyWorld = (EEmptyWorld.IsChecked == true),
                    CpuCountChecked = (ECpuCountCheck.IsChecked == true),
                    MaxMemoryChecked = (EMaxMemoryCheck.IsChecked == true),
                    ExThreadsChecked = (EExThreadsCheck.IsChecked == true),
                    NoSplash = (ESkipIntro.IsChecked == true),
                    ModList = modList,
                    ExThreads = Convert.ToInt32(EExThreads.Text),
                    CpuCount = Convert.ToInt32(ECpuCount.Text),
                    MaxMemory = Convert.ToInt32(EMaxMemory.Text),
                    AdditionalParameters = EAdditionalParameters.Text
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /* UI Event Handlers */
        private void eCpuCountCheck_Checked(object sender, RoutedEventArgs e)
        {
            ECpuCount.IsEnabled = true;
        }

        private void eCpuCountCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ECpuCount.IsEnabled = false;
        }

        private void eMaxMemoryCheck_Checked(object sender, RoutedEventArgs e)
        {
            EMaxMemory.IsEnabled = true;
        }

        private void eMaxMemoryCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            EMaxMemory.IsEnabled = false;
        }

        private void eExThreadsCheck_Checked(object sender, RoutedEventArgs e)
        {
            EExThreads.IsEnabled = true;
        }

        private void eExThreadsCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            EExThreads.IsEnabled = false;
        }
    }
}
