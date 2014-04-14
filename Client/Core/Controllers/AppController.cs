using Client.Core.Classes;
using Client.Core.Enums;
using Client.Core.Utilities;
using Client.Core.Utilities.Classes;
using Client.UI.Windows;
using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Client.Core.Controllers
{
    class AppController
    {
        /* Loggers */
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(AppController));

        /* Constructors */
        public AppController(MainWindow pParent)
        {
            // Save parent for future use.
            this.Parent = pParent;
            this.TokenSource = new CancellationTokenSource();

            // Set current state to INIT.
            SetAppState(AppState.INIT);
        }
        
        /* Fields */
        MainWindow              Parent { get; set; }
        AppState                ApplicationState { get; set; }
        List<Server>            ServerList { get; set; }
        CancellationTokenSource TokenSource { get; set; }
        Server                  CurrentServer { get; set; }

        /* UI Access */
        private void SetAppState(AppState pUIState)
        {
            try
            {
                this.ApplicationState = pUIState;
                Parent.Dispatcher.Invoke(() => Parent.SetUIState(pUIState));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetProgress(Double pProgressValue, String pProgressMessage)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetProgress(pProgressValue, pProgressMessage));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetButtonText(String pButtonText)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetButtonText(pButtonText));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetBrowserTarget(String pUri)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetBrowserTarget(pUri));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetServerList(List<Server> pValues)
        {
            try
            {
                this.ServerList = pValues;

                List<String> Values = new List<string>();

                foreach (Server Value in pValues)
                {
                    Values.Add(Value.Name);
                }

                Parent.Dispatcher.Invoke(() => Parent.SetServerList(Values));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetModFolderList(List<String> pFolderList)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetModFolderList(pFolderList));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetSettings(SettingsCacheEntry pEntry)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetSettings(pEntry));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void GetSettingsRequest()
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.GetSettingsRequest());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /* UI Calls */
        public void ButtonClicked()
        {
            try
            {
                switch (ApplicationState)
                {
                    case AppState.CHECK:
                        TokenSource = new CancellationTokenSource();
                        Task.Run(() => CheckAddons(), TokenSource.Token);
                        break;

                    case AppState.CANCELCHECK:
                        TokenSource.Cancel();
                        SetAppState(AppState.CHECK);
                        break;

                    case AppState.CANCELUPDATE:
                        TokenSource.Cancel();
                        SetAppState(AppState.UPDATE);
                        break;

                    case AppState.UPDATE:
                        TokenSource = new CancellationTokenSource();
                        Task.Run(() => UpdateAddons(), TokenSource.Token);
                        break;

                    case AppState.PLAY:
                        GetSettingsRequest();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ServerSelected(String pSelectedServer)
        {
            try
            {
                CurrentServer = ServerList.Find(p => p.Name == pSelectedServer);

                if (CurrentServer.UpToDate())
                {
                    // All's well
                    ApplicationState = AppState.PLAY;
                    SetAppState(AppState.PLAY);
                }
                else
                {
                    // All's not well
                    // Display update frame
                    ApplicationState = AppState.CHECK;
                    SetAppState(AppState.CHECK);
                    SetBrowserTarget(NetworkUtilities.GetHttpMirror(CurrentServer.ChangelogURLList).Result);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public void Shutdown()
        {
            try
            {
                // Set working directory to executable directory.
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

                // Save the file cache as well as the settings cache.
                LocalMachine.Instance.Save();
                FileCache.Instance.Write();
                SettingsCache.Instance.Write();

                Environment.Exit(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetSettings(SettingsCacheEntry pEntry)
        {
            try
            {
                Task.Run(() => LaunchGame(pEntry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LauncherDisplayed()
        {
            try
            {
                // Get the mod list for this server
                List<String> ModFolderList = new List<string>();

                foreach (String DirectoryPath in Directory.EnumerateDirectories(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), "@*", SearchOption.TopDirectoryOnly))
                {
                    ModFolderList.Add(Path.GetFileName(DirectoryPath));
                }
                SetModFolderList(ModFolderList);

                // Check the settings cache
                if (SettingsCache.Instance.Contains(CurrentServer.IdKey))
                {
                    SetSettings(SettingsCache.Instance.Get(CurrentServer.IdKey));
                }
                else
                {
                    List<String> ModList = new List<string>();

                    foreach (Mod ModEntry in CurrentServer.ModList)
                    {
                        ModList.Add(ModEntry.Name);
                    }

                    // Set default settings
                    SetSettings(new SettingsCacheEntry()
                    {
                        ServerIdKey = CurrentServer.IdKey,
                        ModList = ModList,
                        AdditionalParameters = "-nolog",
                        AutoConnect = true,
                        EmptyWorld = false,
                        NoSplash = true,
                        WinXP = false,
                        Windowed = false,
                        ShowScriptErrors = false,
                        MaxMemoryChecked = false,
                        MaxMemory = 2047,
                        CpuCountChecked = false,
                        CpuCount = 8,
                        ExThreadsChecked = false,
                        ExThreads = 7
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /* Public Methods */
        public async Task InitializeController()
        {
            try
            {
                // Pre-load the server list.
                Task<List<Server>> ServerListTask = GetServerList(Properties.Settings.Default.RemoteServerManifest);

                // Check for updates to the application itself.
                if(await ApplicationUpdater.UpdatesRequired(Properties.Settings.Default.RemoteAppManfest, Properties.Settings.Default.LocalAppManifest))
                {
                    // Update the application.
                    await ApplicationUpdater.Update(Properties.Settings.Default.RemoteAppManfest, Properties.Settings.Default.LocalAppManifest);

                    // Tell this to the user.
                    System.Windows.MessageBox.Show("Application update required.");

                    // Launch the updater.
                    new Process() {StartInfo = new ProcessStartInfo() { FileName = Properties.Settings.Default.ApplicationUpdateClient}}.Start();

                    // Close the application.
                    Environment.Exit(0);
                }
                
                // Set the server list.
                SetServerList(await ServerListTask);
            }
            catch (Exception ex)
            {
                Logger.FatalFormat("Exception has occured while trying to check for application updates.{0}{1}", Environment.NewLine, ex.ToString());
                System.Windows.MessageBox.Show("Problems with the master server =(\nPlease try again later.");
            }
        }

        /* Private Methods */
        private async Task<List<Server>> GetServerList(String pManifestURL)
        {
            try
            {
                return ((Manifest)XMLSerializer.XmlDeserializeFromString
                    (await NetworkUtilities.DownloadToString(pManifestURL), typeof(Manifest))).ServerList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CheckAddons()
        {
            try
            {
                // Check the base and mod directory paths for the selected server.
                if (LocalMachine.Instance.GetBaseDirectory(this.CurrentServer.Type).Equals(String.Empty) || !Directory.Exists(LocalMachine.Instance.GetBaseDirectory(this.CurrentServer.Type)))
                {
                    System.Windows.MessageBox.Show("Please set the base directory for this game.");
                    return;
                }

                if (LocalMachine.Instance.GetModDirectory(this.CurrentServer.Type).Equals(String.Empty) || !Directory.Exists(LocalMachine.Instance.GetModDirectory(this.CurrentServer.Type)))
                {
                    System.Windows.MessageBox.Show("Please set the mod directory for this game.");
                    return;
                }

                // Set app state to CANCELCHECK
                SetAppState(AppState.CANCELCHECK);

                // Receive manifests if needed.
                if (CurrentServer.BaseManifestURL == null)
                {
                    CurrentServer.BaseManifestURL = await NetworkUtilities.GetFtpMirror(CurrentServer.ManifestURLList);
                    await CurrentServer.GetAddonList(CurrentServer.BaseManifestURL);
                }

                // Go through each file and check whether it is present
                Int32 Counter = 0;

                Parallel.ForEach(CurrentServer.AddonList, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = TokenSource.Token }, file =>
                {
                    // Check the status of the addon
                    file.CheckAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), CurrentServer.ConfigExtensionList).Wait();

                    // Update the counter
                    Counter++;

                    // Inform the UI
                    if (!TokenSource.IsCancellationRequested)
                    {
                        this.SetProgress(Counter * (100f / CurrentServer.AddonList.Count), String.Format("Checked {0} of {1} addons.", Counter, CurrentServer.AddonList.Count));
                    }
                });

                    this.SetProgress(100f, "Searching for files to delete...");

                    CurrentServer.FileList = CurrentServer.GetFileList();

                    if(!CurrentServer.UpToDate())
                    {
                        this.ApplicationState = AppState.UPDATE;
                        SetAppState(AppState.UPDATE);

                        this.SetProgress(100f, String.Format("{0} files will be updated, {1} files will be deleted...", CurrentServer.AddonList.Count(p => !p.Status), CurrentServer.FileList.Count));
                    }
                    else
                    {
                        // It's all good.
                        this.ApplicationState = AppState.PLAY;
                        SetAppState(AppState.PLAY);
                    }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateAddons()
        {
            try
            {
                // Set UI state to CANCELUPDATE
                this.ApplicationState = AppState.CANCELUPDATE;
                SetAppState(AppState.CANCELUPDATE);

                Int32 Counter = 0;
                List<Addon> AddonsToDownload = CurrentServer.AddonList.FindAll(p => !p.Status);

                Parallel.ForEach(AddonsToDownload, new ParallelOptions() { MaxDegreeOfParallelism = CurrentServer.ThreadCount, CancellationToken = TokenSource.Token }, file =>
                {
                    // Download the file.
                    file.UpdateAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type)).Wait();
                    // And then check it
                    file.CheckAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), CurrentServer.ConfigExtensionList).Wait();
                    
                    Counter++;

                    // Inform the UI
                    if (!TokenSource.IsCancellationRequested)
                    {
                        this.SetProgress(Counter * (100f / AddonsToDownload.Count), String.Format("Updated {0} of {1} addons.", Counter, AddonsToDownload.Count));
                    }
                });

                this.SetProgress(100f, "Deleting files...");

                // File deletion
                foreach (String Entry in CurrentServer.FileList)
                {
                    if (Directory.Exists(Entry) || File.Exists(Entry))
                    {
                        await FileUtilities.DeleteFile(Entry);
                    }
                }

                // Re-check all of the addons.
                await CheckAddons();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task LaunchGame(SettingsCacheEntry pEntry)
        {
            try
            {
                // Set APP state
                SetAppState(AppState.CLOSE);

                // Save the setting set
                if (SettingsCache.Instance.Contains(CurrentServer.IdKey))
                {
                    SettingsCache.Instance.Update(CurrentServer.IdKey, pEntry);
                }
                else
                {
                    pEntry.ServerIdKey = CurrentServer.IdKey;
                    SettingsCache.Instance.Add(pEntry);
                }

                if (GameLauncher.LaunchGame(this.CurrentServer, pEntry))
                {
                    Shutdown();
                }
                else
                {
                    System.Windows.MessageBox.Show("Unable to launch the game :(");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
