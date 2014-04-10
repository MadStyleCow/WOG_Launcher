using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateClient.UI.Windows;
using UpdateClient.Core.Enums;
using UpdateClient.Core.Classes;
using UpdateClient.Core.Utilities;
using System.Net;
using System.Threading;
using System.IO;
using UpdateClient.Core.Utilities.Classes;
using System.Diagnostics;

namespace UpdateClient.Core.Controllers
{
    class AppController
    {
        /* Constructors */
        public AppController(MainWindow pParent)
        {
            // Save parent for future use.
            this.Parent = pParent;
            this.TokenSource = new CancellationTokenSource();

            // Set current state to INIT.
            this.ApplicationState = AppState.INIT;
            SetUIState(AppState.INIT);
        }
        
        /* Fields */
        MainWindow Parent { get; set; }
        AppState ApplicationState { get; set; }
        List<Server> ServerList { get; set; }
        CancellationTokenSource TokenSource { get; set; }
        Server CurrentServer { get; set; }

        /* UI Access */
        private void SetUIState(AppState pUIState)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetUIState(pUIState));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetProgress(Double pProgressValue, String pProgressMessage)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetProgress(pProgressValue, pProgressMessage));
            }
            catch (Exception)
            {     
                throw;
            }
        }

        private void SetButtonText(String pButtonText)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetButtonText(pButtonText));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetBrowserTarget(String pUri)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetBrowserTarget(pUri));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetServerList(List<Server> pValues)
        {
            try
            {
                List<String> Values = new List<string>();

                foreach (Server Value in pValues)
                {
                    Values.Add(Value.Name);
                }

                Parent.Dispatcher.Invoke(() => Parent.SetServerList(Values));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetModFolderList(List<String> pFolderList)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetModFolderList(pFolderList));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetSettings(SettingsCacheEntry pEntry)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetSettings(pEntry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GetSettingsRequest()
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.GetSettingsRequest());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /* UI Calls */
        public void ButtonClicked()
        {
            try
            {
                if (CurrentServer.GetBaseDirectory() == String.Empty || !Directory.Exists(CurrentServer.GetBaseDirectory()))
                {
                    System.Windows.MessageBox.Show("Please set the base directory for this game.");
                    return;
                }
            
                switch (ApplicationState)
                {
                    case AppState.CHECK:
                        // Get the addon list and the mod list
                        TokenSource = new CancellationTokenSource();
                        Task.Run(() => CheckAddons(), TokenSource.Token);
                        break;

                    case AppState.CANCELCHECK:
                        TokenSource.Cancel();
                        this.ApplicationState = AppState.CHECK;
                        SetUIState(AppState.CHECK);
                        break;

                    case AppState.CANCELUPDATE:
                        TokenSource.Cancel();
                        this.ApplicationState = AppState.UPDATE;
                        SetUIState(AppState.UPDATE);
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
                    SetUIState(AppState.PLAY);
                }
                else
                {
                    // All's not well
                    // Display update frame
                    ApplicationState = AppState.CHECK;
                    SetUIState(AppState.CHECK);
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

                foreach (String DirectoryPath in Directory.EnumerateDirectories(CurrentServer.GetBaseDirectory(), "@*", SearchOption.TopDirectoryOnly))
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
                        AutoConnect = false,
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
                // Launch all of the asynchronous operations
                Task<List<Server>> ServerListTask = Task.Run(() => GetServerList(Properties.Settings.Default.RemoteServerManifest));

                Task<ApplicationManifest> AppManifestTask = Task<ApplicationManifest>.Run<ApplicationManifest>(() =>
                    {
                        return Task<String>.Run<String>(() => NetworkUtilities.DownloadToString(Properties.Settings.Default.RemoteAppManfest))
                            .ContinueWith<ApplicationManifest>(t => (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(t.Result, typeof(ApplicationManifest)));
                    });

                // Check the application for updates
                ApplicationManifest RemoteManifest = await AppManifestTask;

                // Load the local manifest
                ApplicationManifest LocalManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(Properties.Settings.Default.LocalAppManifest, typeof(ApplicationManifest));

                if (!RemoteManifest.ManifestVersion.Equals(LocalManifest.ManifestVersion))
                {
                    foreach (ApplicationFile RemoteFile in RemoteManifest.ApplicationFileList.FindAll(p => p.Type.Equals(FileType.UPDATER)))
                    {
                        if (LocalManifest.ApplicationFileList.Any(p => p.ID.Equals(RemoteFile.ID)))
                        {
                            ApplicationFile LocalFile = LocalManifest.ApplicationFileList.Find(p => p.ID.Equals(RemoteFile.ID));

                            if (File.Exists(RemoteFile.Path))
                            {
                                if (!RemoteFile.Version.Equals(LocalFile.Version))
                                {
                                    FileUtilities.DeleteFile(LocalFile.Path).Wait();
                                    NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                                }
                            }
                            else
                            {
                                NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                            }
                        }
                        else
                        {
                            NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                        }
                    }

                    // In any case, the application needs to be updated.
                    System.Windows.MessageBox.Show("Application requires an update.");

                    new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = Properties.Settings.Default.ApplicationUpdateClient
                        }
                    }.Start();

                    Environment.Exit(0);
                }

                ServerList = await ServerListTask;
                SetServerList(ServerList);
            }
            catch (Exception)
            {               
                throw;
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
                // Set UI state to CANCELCHECK
                ApplicationState = AppState.CANCELCHECK;
                SetUIState(AppState.CANCELCHECK);

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
                    file.CheckAddon(CurrentServer.GetBaseDirectory(), CurrentServer.ConfigExtensionList).Wait();

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
                        SetUIState(AppState.UPDATE);

                        this.SetProgress(100f, String.Format("{0} files will be updated, {1} files will be deleted...", CurrentServer.AddonList.Count(p => !p.Status), CurrentServer.FileList.Count));
                    }
                    else
                    {
                        // It's all good.
                        this.ApplicationState = AppState.PLAY;
                        SetUIState(AppState.PLAY);
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
                SetUIState(AppState.CANCELUPDATE);

                Int32 Counter = 0;
                List<Addon> AddonsToDownload = CurrentServer.AddonList.FindAll(p => !p.Status);

                Parallel.ForEach(AddonsToDownload, new ParallelOptions() { MaxDegreeOfParallelism = CurrentServer.ThreadCount, CancellationToken = TokenSource.Token }, file =>
                {
                    // Download the file.
                    file.UpdateAddon(CurrentServer.GetBaseDirectory()).Wait();

                    // And then check it
                    file.CheckAddon(CurrentServer.GetBaseDirectory(), CurrentServer.ConfigExtensionList).Wait();
                    
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
                this.ApplicationState = AppState.CLOSE;
                SetUIState(AppState.CLOSE);

                // Build a launch string
                StringBuilder LaunchParameters = new StringBuilder();

                if (CurrentServer.Type == GameType.ARMA2OA && CurrentServer.Beta)
                {
                    LaunchParameters.Append(@"-beta=Expansion\Beta;Expansion\Beta\Expansion ");
                }

                if (pEntry.ModList.Count != 0)
                {
                    LaunchParameters.Append("\"-mod=");

                    if (CurrentServer.Type.Equals(GameType.ARMA2OA) && !CurrentServer.GetBaseDirectory().ToLowerInvariant().Equals(Path.GetDirectoryName(Properties.Settings.Default.A2_Path.ToLowerInvariant())))
                    {
                        LaunchParameters.Append(String.Format("{0};Expansion;CA;", Path.GetDirectoryName(Properties.Settings.Default.A2_Path)));
                    }

                    foreach (String Mod in pEntry.ModList)
                    {
                        if (Mod != pEntry.ModList.Last())
                        {
                            LaunchParameters.AppendFormat("{0};", Mod);
                        }
                        else
                        {
                            LaunchParameters.AppendFormat("{0}\" ", Mod);
                        }
                    }
                }

                if (pEntry.NoSplash)
                    LaunchParameters.Append("-nosplash ");

                if (pEntry.ShowScriptErrors)
                    LaunchParameters.Append("-showScriptErrors ");

                if (pEntry.EmptyWorld)
                    LaunchParameters.Append("-world=empty ");

                if (pEntry.Windowed)
                    LaunchParameters.Append("-window ");

                if (pEntry.WinXP)
                    LaunchParameters.Append("-winxp ");

                if (pEntry.CpuCountChecked)
                    LaunchParameters.AppendFormat("-cpuCount={0} ", pEntry.CpuCount);

                if (pEntry.ExThreadsChecked)
                    LaunchParameters.AppendFormat("-exThreads={0} ", pEntry.ExThreads);

                if (pEntry.MaxMemoryChecked)
                    LaunchParameters.AppendFormat("-maxMem={0} ", pEntry.MaxMemory);

                if(pEntry.AutoConnect)
                    LaunchParameters.AppendFormat("-connect={0} -port={1} -password={2} ", CurrentServer.Hostname, CurrentServer.Port, CurrentServer.Password);

                if (pEntry.AdditionalParameters != String.Empty)
                    LaunchParameters.Append(pEntry.AdditionalParameters);

                // Save the setting set
                if(SettingsCache.Instance.Contains(CurrentServer.IdKey))
                {
                    SettingsCache.Instance.Update(CurrentServer.IdKey, pEntry);
                }
                else
                {
                    pEntry.ServerIdKey = CurrentServer.IdKey;
                    SettingsCache.Instance.Add(pEntry);
                }

                // Launch the game
                Process Game = new Process();
                Game.StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = CurrentServer.GetBaseDirectory(),
                    FileName = CurrentServer.GetLaunchPath(),
                    Arguments = LaunchParameters.ToString()
                };
                Game.Start();

                // Save all the required app settings
                Shutdown();

                // Exit the application.
                System.Environment.Exit(0);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
