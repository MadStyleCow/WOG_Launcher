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
using UpdateClient.Model.Utilities.Classes;

namespace UpdateClient.UI.Pages
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Page
    {
        /* Constructors */
        public Launcher()
        {
            InitializeComponent();
        }

        /* UI Access */
        public void SetModFolderList(List<String> pFolderList)
        {
            foreach(String Folder in pFolderList)
            {
                eMods.Items.Add(new CheckBox()
                    {
                        Content = new TextBlock()
                        {
                             Text = Folder
                        }
                    });
            }
        }

        public void SetSettings(SettingsCacheEntry pEntry)
        {
            foreach (String Mod in pEntry.ModList)
            {
                foreach (CheckBox Item in eMods.Items)
                {
                    if (((TextBlock)Item.Content).Text == Mod)
                        Item.IsChecked = true;
                }
            }

            eSkipIntro.IsChecked = pEntry.NoSplash;
            eShowScriptErrors.IsChecked = pEntry.ShowScriptErrors;
            eWinXP.IsChecked = pEntry.WinXP;
            eWindowMode.IsChecked = pEntry.Windowed;
            eEmptyWorld.IsChecked = pEntry.EmptyWorld;
            eCpuCountCheck.IsChecked = pEntry.CpuCountChecked;
            if (eCpuCountCheck.IsChecked == true)
                eCpuCount.Text = pEntry.CpuCount.ToString();
            eMaxMemoryCheck.IsChecked = pEntry.MaxMemoryChecked;
            if (eMaxMemoryCheck.IsChecked == true)
                eMaxMemory.Text = pEntry.MaxMemory.ToString();
            eExThreadsCheck.IsChecked = pEntry.ExThreadsChecked;
            if (eExThreadsCheck.IsChecked == true)
                eExThreads.Text = pEntry.ExThreads.ToString();
            eAdditionalParameters.Text = pEntry.AdditionalParameters;
            eConnect.IsChecked = pEntry.AutoConnect;
        }

        public SettingsCacheEntry GetSettings()
        {
            try
            {
                // Get list of selected mods
                List<String> ModList = new List<string>();

                foreach (CheckBox Item in eMods.Items)
                {
                    if (Item.IsChecked == true)
                    {
                        ModList.Add(((TextBlock)Item.Content).Text);
                    }
                }

                return new SettingsCacheEntry()
                {
                    AutoConnect = (eConnect.IsChecked == true),
                    Windowed = (eWindowMode.IsChecked == true),
                    ShowScriptErrors = (eShowScriptErrors.IsChecked == true),
                    WinXP = (eWinXP.IsChecked == true),
                    EmptyWorld = (eEmptyWorld.IsChecked == true),
                    CpuCountChecked = (eCpuCountCheck.IsChecked == true),
                    MaxMemoryChecked = (eMaxMemoryCheck.IsChecked == true),
                    ExThreadsChecked = (eExThreadsCheck.IsChecked == true),
                    NoSplash = (eSkipIntro.IsChecked == true),
                    ModList = ModList,
                    ExThreads = Convert.ToInt32(eExThreads.Text),
                    CpuCount = Convert.ToInt32(eCpuCount.Text),
                    MaxMemory = Convert.ToInt32(eMaxMemory.Text),
                    AdditionalParameters = eAdditionalParameters.Text
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* UI Event Handlers */
        private void eCpuCountCheck_Checked(object sender, RoutedEventArgs e)
        {
            eCpuCount.IsEnabled = true;
        }

        private void eCpuCountCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            eCpuCount.IsEnabled = false;
        }

        private void eMaxMemoryCheck_Checked(object sender, RoutedEventArgs e)
        {
            eMaxMemory.IsEnabled = true;
        }

        private void eMaxMemoryCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            eMaxMemory.IsEnabled = false;
        }

        private void eExThreadsCheck_Checked(object sender, RoutedEventArgs e)
        {
            eExThreads.IsEnabled = true;
        }

        private void eExThreadsCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            eExThreads.IsEnabled = false;
        }
    }
}
