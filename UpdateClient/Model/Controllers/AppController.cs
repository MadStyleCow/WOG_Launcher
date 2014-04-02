﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateClient.UI.Windows;
using UpdateClient.Model.Enums;
using UpdateClient.Model.Classes;
using UpdateClient.Model.Utilities;
using System.Net;
using System.Threading;
using System.IO;
using UpdateClient.Model.Utilities.Classes;
using System.Diagnostics;

namespace UpdateClient.Model.Controllers
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
            Parent.Dispatcher.Invoke(() => Parent.SetUIState(pUIState));
        }

        private void SetProgress(Double pProgressValue, String pProgressMessage)
        {
            Parent.Dispatcher.Invoke(() => Parent.SetProgress(pProgressValue, pProgressMessage));
        }

        private void SetButtonText(String pButtonText)
        {
            Parent.Dispatcher.Invoke(() => Parent.SetButtonText(pButtonText));
        }

        private void SetBrowserTarget(String pUri)
        {
            Parent.Dispatcher.Invoke(() => Parent.SetBrowserTarget(pUri));
        }

        private void SetServerList(List<Server> pValues)
        {
            List<String> Values = new List<string>();

            foreach(Server Value in pValues)
            {
                Values.Add(Value.Name);
            }

            Parent.Dispatcher.Invoke(() => Parent.SetServerList(Values));
        }

        private void SetModFolderList(List<String> pFolderList)
        {
            Parent.Dispatcher.Invoke(() => Parent.SetModFolderList(pFolderList));
        }

        private void SetSettings(SettingsCacheEntry pEntry)
        {
            Parent.Dispatcher.Invoke(() => Parent.SetSettings(pEntry));
        }

        private void GetSettingsRequest()
        {
            Parent.Dispatcher.Invoke(() => Parent.GetSettingsRequest());
        }

        /* UI Calls */
        public void ButtonClicked()
        {
            if(CurrentServer.GetBaseDirectory() == String.Empty || !Directory.Exists(CurrentServer.GetBaseDirectory()))
            {
                System.Windows.MessageBox.Show("Please set the base directory for this game.");
                return;
            }

            try
            {
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
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public void ServerSelected(String pSelectedServer)
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
                SetBrowserTarget(CurrentServer.GetChangelogURL().Result);
            }
        }

        public void Shutdown()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            FileCache.Instance.Write();
            SettingsCache.Instance.Write(Properties.Settings.Default.SettingsCache);
        }

        public void GetSettings(SettingsCacheEntry pEntry)
        {
            Task.Run(() => LaunchGame(pEntry));
        }

        public void LauncherDisplayed()
        {
            // Get the mod list for this server
            List<String> ModFolderList = new List<string>();

            foreach(String DirectoryPath in Directory.EnumerateDirectories(CurrentServer.GetBaseDirectory(), "@*", SearchOption.TopDirectoryOnly))
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
                
                foreach(Mod ModEntry in CurrentServer.ModList)
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

        /* Public Methods */
        public async Task InitializeController()
        {
            ServerList = await Task.Run(() => GetServerList(Properties.Settings.Default.ManifestURL));

            SetServerList(ServerList);
        }

        /* Private Methods */
        private async Task<List<Server>> GetServerList(String pManifestURL)
        {
            try
            {
                return ((Manifest)XMLSerializer.XmlDeserializeFromString
                    (new WebClient().DownloadString(pManifestURL), typeof(Manifest))).ServerList;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        private async Task CheckAddons()
        {
            try
            {
                // Get manifest
                if (CurrentServer.BaseManifestURL == null)
                {
                    CurrentServer.BaseManifestURL = Task.Run(() => CurrentServer.GetManifestURL(), TokenSource.Token).Result;

                    // Get addon and mod lists
                    await Task.Run(() => CurrentServer.GetAddonList(CurrentServer.BaseManifestURL), TokenSource.Token);
                }

                // Go through each file and check whether it is present
                Int32 Counter = 0;

                FileCache.Instance = new FileCache();

                Task.Factory.StartNew(() =>
                {
                    ApplicationState = AppState.CANCELCHECK;
                    SetUIState(AppState.CANCELCHECK);

                    Parallel.ForEach(CurrentServer.AddonList, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, file =>
                    {
                        Task<Boolean>.Run(() => file.CheckAddon(CurrentServer.GetBaseDirectory(), CurrentServer.ConfigExtensionList), TokenSource.Token).Wait();

                        Counter++;

                        // Inform the UI
                        if(!TokenSource.IsCancellationRequested)
                            this.SetProgress(Counter * (100f / CurrentServer.AddonList.Count), String.Format("Checked {0} of {1} addons.", Counter, CurrentServer.AddonList.Count));
                    });

                    FileCache.Instance.Write();

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
                }, TokenSource.Token);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private async Task UpdateAddons()
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    this.ApplicationState = AppState.CANCELUPDATE;
                    SetUIState(AppState.CANCELUPDATE);

                    Int32 Counter = 0;

                    Parallel.ForEach(CurrentServer.AddonList.FindAll(p => !p.Status), new ParallelOptions() { MaxDegreeOfParallelism = CurrentServer.ThreadCount }, file =>
                    {
                        Task<Boolean>.Run(() => file.UpdateAddon(CurrentServer.GetBaseDirectory()), TokenSource.Token).Wait();

                        Counter++;

                        // Inform the UI
                        if(!TokenSource.IsCancellationRequested)
                            this.SetProgress(Counter * (100f / CurrentServer.AddonList.Count(p => !p.Status)), String.Format("Updated {0} of {1} addons.", Counter, CurrentServer.AddonList.Count(p => !p.Status)));
                    });

                    this.SetProgress(100f, "Deleting files...");
                    
                    // File deletion
                    foreach(String File in CurrentServer.FileList)
                    {
                        if(System.IO.File.Exists(File))
                            FileUtilities.DeleteFile(File);
                    }

                    // Re-check all of the addons.
                    Task.Run(() => CheckAddons(), TokenSource.Token);
                }, TokenSource.Token);   
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
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

                if(pEntry.ModList.Count != 0)
                {
                    LaunchParameters.Append("-mod=");
                    foreach(String Mod in pEntry.ModList)
                    {
                        if (Mod != pEntry.ModList.Last())
                            LaunchParameters.AppendFormat("{0};", Mod);
                        else
                            LaunchParameters.AppendFormat("{0} ", Mod);
                    }
                }

                if(CurrentServer.Beta)
                    LaunchParameters.Append(@"-beta=Expansion\Beta;Expansion\Beta\Expansion ");
                
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
