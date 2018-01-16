using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Client.Core.Classes;
using Client.Core.Enums;
using Client.Core.Utilities;
using Client.Core.Utilities.Classes;
using Client.UI.Windows;
using log4net;
using Settings = Client.Properties.Settings;

namespace Client.Core.Controllers
{
    class AppController
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AppController));

        /* Constructors */
        public AppController(MainWindow pParent)
        {
            // Save parent for future use.
            Parent = pParent;
            TokenSource = new CancellationTokenSource();

            Logger.InfoFormat("Application start-up. OS: {0} CpuCount {1}", LocalMachine.Instance.Os, LocalMachine.Instance.CpuCount);

            // Set current state to INIT.
            SetApplicationState(AppState.Init);
        }
        
        /* Fields */
        MainWindow              Parent { get; set; }
        AppState                ApplicationState { get; set; }
        List<Server>            ServerList { get; set; }
        CancellationTokenSource TokenSource { get; set; }
        Server                  CurrentServer { get; set; }

        /* UI Access */
        private void SetApplicationState(AppState pUiState)
        {
            try
            {
                ApplicationState = pUiState;
                Parent.Dispatcher.Invoke(() => Parent.SetUiState(pUiState));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Logger.Fatal("Failed to set application state", ex);
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
                MessageBox.Show(ex.ToString());
                Logger.Error("Failed to set progress value", ex);
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
                MessageBox.Show(ex.ToString());
                Logger.Error("Failed to set browser target", ex);
            }
        }

        private void SetServerList(List<Server> pValues)
        {
            try
            {
                ServerList = pValues;

                var values = pValues.Select(value => value.Name).ToList();

                Parent.Dispatcher.Invoke(() => Parent.SetServerList(values));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SetModFolderList(IEnumerable<string> pFolderList)
        {
            try
            {
                Parent.Dispatcher.Invoke(() => Parent.SetModFolderList(pFolderList));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                MessageBox.Show(ex.ToString());
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
                MessageBox.Show(ex.ToString());
            }
        }

        /* UI Calls */
        public void ButtonClicked()
        {
            try
            {
                // Check the paths.
                if (!LocalMachine.Instance.PathsSet(CurrentServer.Type))
                {
                    MessageBox.Show("Please set the game and mod paths for this game.\nIf the game is a steam version - please set the steam path as well.");
                    return;
                }

                switch (ApplicationState)
                {
                    case AppState.Check:
                        TokenSource = new CancellationTokenSource();
                        Task.Run(() => CheckAddons(), TokenSource.Token);
                        break;

                    case AppState.Cancelcheck:
                        TokenSource.Cancel();
                        SetApplicationState(AppState.Check);
                        break;

                    case AppState.Cancelupdate:
                        TokenSource.Cancel();
                        SetApplicationState(AppState.Update);
                        break;

                    case AppState.Update:
                        TokenSource = new CancellationTokenSource();
                        Task.Run(() => UpdateAddons(), TokenSource.Token);
                        break;

                    case AppState.Play:
                        GetSettingsRequest();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while pushing a button.", ex);
                throw;
            }
        }

        public void ServerSelected(String pSelectedServer)
        {
            try
            {
                CurrentServer = ServerList.Find(p => p.Name == pSelectedServer);

                if (CurrentServer.AddonSetValid().Result)
                {
                    // All's well
                    SetApplicationState(AppState.Play);
                }
                else
                {
                    // All's not well
                    // Display update frame
                    SetApplicationState(AppState.Check);
                    SetBrowserTarget(NetworkUtilities.GetMirror(CurrentServer.ChangelogUrlList));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Shutdown()
        {
            try
            {
                // Set working directory to executable directory.
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                // Save the file cache as well as the settings cache.
                FileCache.Instance.Write();
                SettingsCache.Instance.Write();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to save the caches.", ex);
                throw;
            }
        }

        public void GetSettings(SettingsCacheEntry pEntry)
        {
            try
            {
                Task.Run(() => LaunchGame(pEntry));
            }
            catch (Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to launch the game.", ex);
                throw;
            }
        }

        public void LauncherDisplayed()
        {
            try
            {
                // Get the mod list for this server
                SetModFolderList(Directory.EnumerateDirectories(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), "@*", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToList());

                // Check the settings cache
                if (SettingsCache.Instance.Contains(CurrentServer.IdKey))
                {
                    SetSettings(SettingsCache.Instance.Get(CurrentServer.IdKey));
                }
                else
                {
                    var modList = CurrentServer.ModList.Select(modEntry => modEntry.Name).ToList();

                    // Set default settings
                    SetSettings(new SettingsCacheEntry
                    {
                        ServerIdKey = CurrentServer.IdKey,
                        ModList = modList,
                        AdditionalParameters = "-nolog",
                        AutoConnect = true,
                        EmptyWorld = false,
                        NoSplash = true,
                        WinXp = false,
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
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to display the launcher", ex);
                throw;
            }
        }

        /* Public Methods */
        public async Task InitializeController()
        {
            try
            {
                // Pre-load the server list.
                var serverListTask = GetServerList(Settings.Default.RemoteServerManifest);

                // Check for updates to the application itself.
                if(await ApplicationUpdater.UpdatesRequired(Settings.Default.RemoteAppManfest, Settings.Default.LocalAppManifest))
                {
                    // Update the application.
                    await ApplicationUpdater.Update(Settings.Default.RemoteAppManfest, Settings.Default.LocalAppManifest);

                    // Tell this to the user.
                    MessageBox.Show("Application update required.");

                    // Launch the updater.
                    new Process {StartInfo = new ProcessStartInfo { FileName = Settings.Default.ApplicationUpdateClient}}.Start();

                    // Close the application.
                    Environment.Exit(0);
                }
                
                // Set the server list.
                SetServerList(await serverListTask);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Exception has occured while trying to check for application updates.", ex);
                MessageBox.Show("Problems with the master server =(\nPlease try again later.");
            }
        }

        /* Private Methods */
        private async Task<List<Server>> GetServerList(String pManifestUrl)
        {
            try
            {
                return ((Manifest)XMLSerializer.XmlDeserializeFromString
                    (await NetworkUtilities.DownloadToString(pManifestUrl), typeof(Manifest))).ServerList;
            }
            catch (Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to receive the server list.", ex);
                throw;
            }
        }

        private async Task CheckAddons()
        {
            /*
             * TODO LIST
             * 1.) General clutter of the code. Basically - it looks ugly. Try to optimize / make better.
             * Perhaps even rewrite some pieces of code. Especially on lower levels.
             */

            try
            {
                if (LocalMachine.Instance.PathsSet(CurrentServer.Type))
                {
                    SetApplicationState(AppState.Cancelcheck);
                    // Receive manifests if needed.
                    if (CurrentServer.BaseManifestUrl == null)
                    {
                        CurrentServer.BaseManifestUrl = NetworkUtilities.GetMirror(CurrentServer.ManifestUrlList);
                        await CurrentServer.GetData(CurrentServer.BaseManifestUrl);
                    }

                    // Go through each file and check whether it is present
                    var processedAddons = 0;
                    Parallel.ForEach(CurrentServer.AddonList,
                        new ParallelOptions
                        {
                            MaxDegreeOfParallelism = Environment.ProcessorCount,
                            CancellationToken = TokenSource.Token
                        }, processableAddon =>
                        {
                            processableAddon.CheckAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type),
                                CurrentServer.ConfigExtensionList).Wait();
                            processedAddons++;
                            if (!TokenSource.IsCancellationRequested)
                            {
                                SetProgress(processedAddons*(100f/CurrentServer.AddonList.Count),
                                    String.Format("Checked {0} of {1} addons.", processedAddons,
                                        CurrentServer.AddonList.Count));
                            }
                        });

                    SetProgress(100f, "Searching for files to delete...");
                    CurrentServer.FileList = await CurrentServer.GetFileDeleteList();
                    if (!await CurrentServer.AddonSetValid())
                    {
                        SetApplicationState(AppState.Update);

                        SetProgress(100f,
                            String.Format("{0} files will be updated ({1} MB), {2} files will be deleted...",
                                CurrentServer.AddonList.Count(p => !p.Status),
                                (CurrentServer.AddonList.FindAll(p => !p.Status).Sum(p => p.Size)/1024/1024),
                                CurrentServer.FileList.Count));
                    }
                    else
                    {
                        SetApplicationState(AppState.Play);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please set the game and mod paths for this game.\nIf the game is a steam version - please set the steam path as well.");
                }
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (ApplicationException ex)
            {
                // No mirrors are currently available.
                Logger.Error("An error occured while trying to locate a mirror.", ex);
                SetProgress(0, "No mirrors available...");
                MessageBox.Show("No mirrors available...");
                SetApplicationState(AppState.Play);
            }
            catch (Exception ex)
            {
                Logger.Fatal("An error occured while checking the addon-set.", ex);
                SetProgress(0, "An error occured while trying to check for addons.");
                MessageBox.Show("An error occured while trying to check for addons.");
                SetApplicationState(AppState.Check);
            }
        }

        private async Task UpdateAddons()
        {
            try
            {
                // Set UI state to CANCELUPDATE
                SetApplicationState(AppState.Cancelupdate);

                var counter = 0;
                var addonsToDownload = CurrentServer.AddonList.FindAll(p => !p.Status);

                Parallel.ForEach(addonsToDownload, new ParallelOptions { MaxDegreeOfParallelism = CurrentServer.ThreadCount, CancellationToken = TokenSource.Token }, file =>
                {
                    if (!file.UpdateAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), CurrentServer.BaseManifestUrl.Substring(0, CurrentServer.BaseManifestUrl.LastIndexOf("/", StringComparison.Ordinal))).Result)
                    {

                        // Get a new base manifest URL
                        String newBaseManifestUrl = NetworkUtilities.GetMirror(CurrentServer.ManifestUrlList);
                        CurrentServer.BaseManifestUrl = newBaseManifestUrl;

                        // Repeat
                        file.UpdateAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), CurrentServer.BaseManifestUrl.Substring(0, CurrentServer.BaseManifestUrl.LastIndexOf("/", StringComparison.Ordinal))).Wait(); 
                    }

                    // And then check it
                    file.CheckAddon(LocalMachine.Instance.GetModDirectory(CurrentServer.Type), CurrentServer.ConfigExtensionList).Wait();
                    
                    counter++;

                    // Inform the UI
                    if (!TokenSource.IsCancellationRequested)
                    {
                        SetProgress(counter * (100f / addonsToDownload.Count), String.Format("Updated {0} ({1} MB) of {2} ({3} MB) addons.", counter, addonsToDownload.FindAll(p => p.Status).Sum(p => p.Size) / 1024 / 1024, addonsToDownload.Count, addonsToDownload.Sum(p => p.Size) / 1024 / 1024));
                    }
                });

                SetProgress(100f, "Deleting files...");

                // File deletion
                // Order a lot of file deletions and wait for them to complete.
                List<Task> DeleteTasks = new List<Task>();
                foreach (String Entry in CurrentServer.FileList)
                {
                    if (Directory.Exists(Entry) || File.Exists(Entry))
                    {
                        DeleteTasks.Add(FileUtilities.DeleteFile(Entry));
                    }
                }
                await Task.WhenAll(DeleteTasks);

                // Re-check all of the addons (just to be sure).
                await CheckAddons();
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                SetApplicationState(AppState.Update);
                Logger.Fatal("An error was encountered while trying to update the addon set.", ex);
                throw;
            }
        }

        private async Task LaunchGame(SettingsCacheEntry pEntry)
        {
            try
            {
                // Set APP state
                SetApplicationState(AppState.Close);

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

                if (GameLauncher.LaunchGame(CurrentServer, pEntry))
                {
                    Shutdown();
                }
                else
                {
                    MessageBox.Show("Unable to launch the game :(");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
