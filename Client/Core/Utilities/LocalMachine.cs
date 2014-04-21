using System;
using System.Collections.Generic;
using System.IO;
using Client.Core.Enums;
using Client.Properties;
using log4net;
using Microsoft.Win32;

namespace Client.Core.Utilities
{
    public class LocalMachine
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ApplicationUpdater));

        /* Static fields */
        public static readonly LocalMachine Instance = new LocalMachine();

        /* Public fields */
        public OperatingSystem Os   { get; private set; }
        public Int32 CpuCount       { get; private set; }

        // Paths should store the way to the executable
        public String A2Path        { get; set; }
        public String A2OaPath      { get; set; }    
        public String A2OaBetaPath  { get; set; }
        public String A3Path        { get; set; }        
        public String SteamPath     { get; set; }

        // Paths should store only the folder
        public String A2AddonPath   { get; set; }
        public String A2OaAddonPath { get; set; }
        public String A3AddonPath   { get; set; }

        /* Private fields */
        readonly List<String> _steamRegistryKeys  = new List<string> { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam",                           @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam"};
        readonly List<String> _a2RegistryKeys     = new List<string> { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2",     @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2"};
        readonly List<String> _a2OaRegistryKeys   = new List<string> { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA",  @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA" };
        readonly List<String> _a3RegistryKeys     = new List<string> { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive\arma 3",            @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive\arma 3" };

        /* Constructors */
        private LocalMachine()
        {
            try
            {
                // Assign local machine parameters
                Os = Environment.OSVersion;
                CpuCount = Environment.ProcessorCount;

                // Assign paths
                SteamPath = LocateExecutablePath(GameType.Steam);
                A2Path = LocateExecutablePath(GameType.Arma2);
                A2AddonPath = LocateModPath(GameType.Arma2);
                A2OaPath = LocateExecutablePath(GameType.Arma2Oa);
                A2OaAddonPath = LocateModPath(GameType.Arma2Oa);
                A2OaBetaPath = LocateExecutablePath(GameType.Arma2Oabeta);
                A3Path = LocateExecutablePath(GameType.Arma3);
                A3AddonPath = LocateModPath(GameType.Arma3);
            }
            catch (Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to construct the LocalMachine class.", ex);
                throw;
            }
        }

        /* Private methods */
        private string LocateExecutablePath(GameType pGameType)
        {
            try
            {
                switch(pGameType)
                {
                    case GameType.Steam:
                        if(!Settings.Default.Steam_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.Steam_Path;
                        }
                        foreach (var registryKey in _steamRegistryKeys)
                        {
                            var keyValue = (String)Registry.GetValue(registryKey, "InstallPath", "");
                            if (keyValue != null)
                            {
                                return Path.Combine(keyValue, "steam.exe");
                            }
                        }
                        return String.Empty;

                    case GameType.Arma2:
                        if(!Settings.Default.A2_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A2_Path;
                        }
                        foreach (var registryKey in _a2RegistryKeys)
                        {
                            var keyValue = (String)Registry.GetValue(registryKey, "main", "");
                            if (keyValue != null)
                            {
                                return Path.Combine(keyValue, "arma2.exe");
                            }
                        }
                        return String.Empty;

                    case GameType.Arma2Oa:
                        if(!Settings.Default.A2OA_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A2OA_Path;
                        }
                        foreach (var registryKey in _a2OaRegistryKeys)
                        {
                            var keyValue = (String)Registry.GetValue(registryKey, "main", "");
                            if (keyValue != null)
                            {
                                return Path.Combine(keyValue, "arma2oa.exe");
                            }
                        }
                        return String.Empty;

                    case GameType.Arma2Oabeta:
                        if(!Settings.Default.A2OABeta_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A2OABeta_Path;
                        }
                        if (!A2OaAddonPath.Equals(String.Empty))
                        {
                            return Path.Combine(A2OaAddonPath, @"Expansion\Beta\Arma2oa.exe");
                        }
                        return String.Empty;

                    case GameType.Arma3:
                        if(!Settings.Default.A3_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A3_Path;
                        }
                        foreach (var registryKey in _a3RegistryKeys)
                        {
                            var keyValue = (String)Registry.GetValue(registryKey, "main", "");
                            if (keyValue != null)
                            {
                                return Path.Combine(keyValue, "arma3.exe");
                            }
                        }
                        return String.Empty;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to locate an executable path.", ex);
                throw;
            }
        }

        private String LocateModPath(GameType pGameType)
        {
            try
            {
                switch(pGameType)
                {
                    case GameType.Arma2:
                        if (!Settings.Default.A2_AddonPath.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A2_AddonPath;
                        }
                        if (!A2Path.Equals(String.Empty))
                        {
                            return Path.GetDirectoryName(A2Path);
                        }
                        return String.Empty;

                    case GameType.Arma2Oa:
                    case GameType.Arma2Oabeta:
                        if (!Settings.Default.A2OA_Addon_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A2OA_Addon_Path;
                        }
                        if (!A2OaPath.Equals(String.Empty))
                        {
                            return Path.GetDirectoryName(A2OaPath);
                        }
                        return String.Empty;

                    case GameType.Arma3:
                        if (!Settings.Default.A3_Addon_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Settings.Default.A3_Addon_Path;
                        }
                        if (!A3Path.Equals(String.Empty))
                        {
                            return Path.GetDirectoryName(A3Path);
                        }
                        return String.Empty;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to locate a mod path.", ex);
                throw;
            }
        }

        /* Public methods */
        /// <summary>
        /// Returns a base directory for a given game type.
        /// </summary>
        /// <param name="pGameType">Game type, for which the base directory should be returned.</param>
        /// <returns>Game base directory path.</returns>
        public string GetBaseDirectory(GameType pGameType)
        {
            try
            {
                switch(pGameType)
                {
<<<<<<< HEAD
                    case GameType.Steam:
                        return Path.GetDirectoryName(SteamPath);
                    case GameType.Arma2:
                        return Path.GetDirectoryName(A2Path);
                    case GameType.Arma2Oa:
                        return Path.GetDirectoryName(A2OaPath);
                    case GameType.Arma2Oabeta:
                        return Path.GetDirectoryName(A2OaBetaPath);
                    case GameType.Arma3:
                        return Path.GetDirectoryName(A3Path);
=======
                    case GameType.STEAM:
                        return !SteamPath.Equals(String.Empty) ? Path.GetDirectoryName(SteamPath) : String.Empty;
                    case GameType.ARMA2:
                        return !A2Path.Equals(String.Empty) ? Path.GetDirectoryName(A2Path) : String.Empty;
                    case GameType.ARMA2OA:
                        return !A2OAPath.Equals(String.Empty) ? Path.GetDirectoryName(A2OAPath) : String.Empty;
                    case GameType.ARMA2OABETA:
                        return !A2OABetaPath.Equals(String.Empty) ? Path.GetDirectoryName(A2OABetaPath) : String.Empty;
                    case GameType.ARMA3:
                        return !A3Path.Equals(String.Empty) ? Path.GetDirectoryName(A3Path) : String.Empty;
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15
                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to get the base directory.", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns a mod directory for the indicated game type.
        /// </summary>
        /// <param name="pGameType">Game type, for which the mod directory should be returned.</param>
        /// <returns>Game mod directory path.</returns>
        public string GetModDirectory(GameType pGameType)
        {
            try
            {
                switch(pGameType)
                {
                    case GameType.Arma2:
                        return A2AddonPath;

                    case GameType.Arma2Oa:
                    case GameType.Arma2Oabeta:
                        return A2OaAddonPath;

                    case GameType.Arma3:
                        return A3AddonPath;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to get the mod directory.", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns the executable for the given game type.
        /// </summary>
        /// <param name="pGameType">Game type, for which the executable should be returned.</param>
        /// <returns>Game executable path.</returns>
        public string GetExecutable(GameType pGameType)
        {
            try
            {
                switch (pGameType)
                {
                    case GameType.Steam:
                        return SteamPath;
                    case GameType.Arma2:
                        return A2Path;
                    case GameType.Arma2Oa:
                        return A2OaPath;
                    case GameType.Arma2Oabeta:
                        return A2OaBetaPath;
                    case GameType.Arma3:
                        return A3Path;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to get the executable path.", ex);
                throw;
            }
        }

        /// <summary>
        /// Indicates whether the indicated gametype is a steam version.
        /// </summary>
        /// <param name="pGameType">Game type, for which the steam version status should be returned.</param>
        /// <returns>Steam status</returns>
        public bool SteamVersion(GameType pGameType)
        {
            try
            {
                switch (pGameType)
                {
<<<<<<< HEAD
                    case GameType.Arma2:
                        if (!GetBaseDirectory(GameType.Arma2).Equals(String.Empty))
                        {
                            return Directory.GetFiles(GetBaseDirectory(GameType.Arma2)).Length != 0;
=======
                    case GameType.ARMA2:
                        if (!GetBaseDirectory(GameType.ARMA2).Equals(String.Empty))
                        {
                            return Directory.GetFiles(GetBaseDirectory(GameType.ARMA2), "*.vdf", SearchOption.AllDirectories).Length != 0;
                        }
                        else
                        {
                            return false;
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15
                        }
                        throw new ArgumentNullException();

                    case GameType.Arma2Oa:
                    case GameType.Arma2Oabeta:
                        if (!GetBaseDirectory(GameType.Arma2Oa).Equals(String.Empty))
                        {
<<<<<<< HEAD
                            return Directory.GetFiles(GetBaseDirectory(GameType.Arma2Oa)).Length != 0;
=======
                            return Directory.GetFiles(GetBaseDirectory(GameType.ARMA2OA), "*.vdf", SearchOption.AllDirectories).Length != 0;
                        }
                        else
                        {
                            return false;
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15
                        }
                        throw new ArgumentNullException();

                    case GameType.Arma3:
                        return true;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to assert steam version.", ex);
                throw;
            }
        }

        public bool PathsSet(GameType pGameType)
        {
            try
            {
                switch (pGameType)
                {
                    case GameType.Arma2:
                        if (SteamVersion(GameType.Arma2))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2AddonPath.Equals(String.Empty));
                        }
                        return (!A2Path.Equals(String.Empty));

                    case GameType.Arma2Oa:
                        if(SteamVersion(GameType.Arma2Oa))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2OaPath.Equals(String.Empty) && !A2OaAddonPath.Equals(String.Empty));
                        }
                        return (!A2OaPath.Equals(String.Empty) && !A2OaAddonPath.Equals(String.Empty));

                    case GameType.Arma2Oabeta:
                        if (SteamVersion(GameType.Arma2Oa))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2OaPath.Equals(String.Empty) && !A2OaAddonPath.Equals(String.Empty) && !A2OaBetaPath.Equals(String.Empty));
                        }
                        return (!A2OaPath.Equals(String.Empty) && !A2OaAddonPath.Equals(String.Empty));

                    case GameType.Arma3:
                        if (SteamVersion(GameType.Arma3))
                        {
                            return (!A3Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A3AddonPath.Equals(String.Empty));
                        }
<<<<<<< HEAD
                        // Never gonna happen.
                        return (!A3Path.Equals(String.Empty) && !A3AddonPath.Equals(String.Empty));
=======
                        else
                        {
                            // Never gonna happen.
                            return (!A3Path.Equals(String.Empty) && !A3AddonPath.Equals(String.Empty));
                        }
>>>>>>> bf267f3e6e2eef82ba50a14bed8e67c82da24a15

                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to assert whether the paths are set.", ex);
                throw;
            }
        }

        /// <summary>
        /// Saves the local machine paths into the user settings, in order to allow them to be reused on next launch.
        /// </summary>
        public void Save()
        {
            try
            {
                Settings.Default.Steam_Path      = SteamPath;
                Settings.Default.A2_Path         = A2Path;
                Settings.Default.A2OA_Path       = A2OaPath;
                Settings.Default.A2OABeta_Path   = A2OaBetaPath;
                Settings.Default.A3_Path         = A3Path;
                Settings.Default.A2OA_Addon_Path = A2OaAddonPath;
                Settings.Default.A3_Addon_Path   = A3AddonPath;

                Settings.Default.Save();
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to save the paths.", ex);
                throw;
            }
        }
    }
}
