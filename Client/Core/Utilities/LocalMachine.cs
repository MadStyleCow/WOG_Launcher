using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Client.Core.Enums;

namespace Client.Core.Utilities
{
    public class LocalMachine
    {
        /* Static fields */
        public static LocalMachine Instance = new LocalMachine();

        /* Public fields */
        public OperatingSystem OS   { get; private set; }
        public Int32 CpuCount       { get; private set; }

        // Paths should store the way to the executable
        public String A2Path        { get; set; }
        public String A2OAPath      { get; set; }    
        public String A2OABetaPath  { get; set; }
        public String A3Path        { get; set; }        
        public String SteamPath     { get; set; }

        // Paths should store only the folder
        public String A2AddonPath   { get; set; }
        public String A2OAAddonPath { get; set; }
        public String A3AddonPath   { get; set; }

        /* Private fields */
        List<String> SteamRegistryKeys  = new List<string>()    { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam",                           @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam"};
        List<String> A2RegistryKeys     = new List<string>()    { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2",     @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2"};
        List<String> A2OARegistryKeys   = new List<string>()    { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA",  @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA" };
        List<String> A3RegistryKeys     = new List<string>()    { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive\arma 3",            @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive\arma 3" };

        /* Constructors */
        public LocalMachine()
        {
            try
            {
                // Assign local machine parameters
                this.OS = Environment.OSVersion;
                this.CpuCount = Environment.ProcessorCount;

                // Assign paths
                this.SteamPath = LocateExecutablePath(GameType.STEAM);
                this.A2Path = LocateExecutablePath(GameType.ARMA2);
                this.A2AddonPath = LocateModPath(GameType.ARMA2);
                this.A2OAPath = LocateExecutablePath(GameType.ARMA2OA);
                this.A2OAAddonPath = LocateModPath(GameType.ARMA2OA);
                this.A2OABetaPath = LocateExecutablePath(GameType.ARMA2OABETA);
                this.A3Path = LocateExecutablePath(GameType.ARMA3);
                this.A3AddonPath = LocateModPath(GameType.ARMA3);
            }
            catch (Exception)
            {
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
                    case GameType.STEAM:
                        if(!Properties.Settings.Default.Steam_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.Steam_Path;
                        }
                        else
                        {
                            foreach (String RegistryKey in SteamRegistryKeys)
                            {
                                String KeyValue = (String)Registry.GetValue(RegistryKey, "InstallPath", "");
                                if (KeyValue != null)
                                {
                                    return Path.Combine(KeyValue, "steam.exe");
                                }
                            }
                            return String.Empty;
                        }

                    case GameType.ARMA2:
                        if(!Properties.Settings.Default.A2_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2_Path;
                        }
                        else
                        {
                            foreach (String RegistryKey in A2RegistryKeys)
                            {
                                String KeyValue = (String)Registry.GetValue(RegistryKey, "main", "");
                                if (KeyValue != null)
                                {
                                    return Path.Combine(KeyValue, "arma2.exe");
                                }
                            }
                            return String.Empty;
                        }

                    case GameType.ARMA2OA:
                        if(!Properties.Settings.Default.A2OA_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2OA_Path;
                        }
                        else
                        {
                            foreach (String RegistryKey in A2OARegistryKeys)
                            {
                                String KeyValue = (String)Registry.GetValue(RegistryKey, "main", "");
                                if (KeyValue != null)
                                {
                                    return Path.Combine(KeyValue, "arma2oa.exe");
                                }
                            }
                            return String.Empty;
                        }

                    case GameType.ARMA2OABETA:
                        if(!Properties.Settings.Default.A2OABeta_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2OABeta_Path;
                        }
                        else
                        {
                            if (!this.A2OAAddonPath.Equals(String.Empty))
                            {
                                return Path.Combine(this.A2OAAddonPath, @"Expansion\Beta\Arma2oa.exe");
                            }
                            else
                            {
                                return String.Empty;
                            }
                        }

                    case GameType.ARMA3:
                        if(!Properties.Settings.Default.A3_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A3_Path;
                        }
                        else
                        {
                            foreach (String RegistryKey in A3RegistryKeys)
                            {
                                String KeyValue = (String)Registry.GetValue(RegistryKey, "main", "");
                                if (KeyValue != null)
                                {
                                    return Path.Combine(KeyValue, "arma3.exe");
                                }
                            }
                            return String.Empty;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private String LocateModPath(GameType pGameType)
        {
            try
            {
                switch(pGameType)
                {
                    case GameType.ARMA2:
                        if (!Properties.Settings.Default.A2_AddonPath.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2_AddonPath;
                        }
                        else
                        {
                            if (!this.A2Path.Equals(String.Empty))
                            {
                                return Path.GetDirectoryName(this.A2Path);
                            }
                            else
                            {
                                return String.Empty;
                            }
                        }
                        
                    case GameType.ARMA2OA:
                    case GameType.ARMA2OABETA:
                        if (!Properties.Settings.Default.A2OA_Addon_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2OA_Addon_Path;
                        }
                        else
                        {
                            if (!this.A2OAPath.Equals(String.Empty))
                            {
                                return Path.GetDirectoryName(this.A2OAPath);
                            }
                            else
                            {
                                return String.Empty;
                            }
                        }

                    case GameType.ARMA3:
                        if (!Properties.Settings.Default.A3_Addon_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A3_Addon_Path;
                        }
                        else
                        {
                            if (!this.A3Path.Equals(String.Empty))
                            {
                                return Path.GetDirectoryName(this.A3Path);
                            }
                            else
                            {
                                return String.Empty;
                            }
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception)
            {
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
                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception)
            {
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
                    case GameType.ARMA2:
                        return A2AddonPath;

                    case GameType.ARMA2OA:
                    case GameType.ARMA2OABETA:
                        return A2OAAddonPath;

                    case GameType.ARMA3:
                        return A3AddonPath;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
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
                    case GameType.STEAM:
                        return SteamPath;
                    case GameType.ARMA2:
                        return A2Path;
                    case GameType.ARMA2OA:
                        return A2OAPath;
                    case GameType.ARMA2OABETA:
                        return A2OABetaPath;
                    case GameType.ARMA3:
                        return A3Path;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
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
                    case GameType.ARMA2:
                        if (!GetBaseDirectory(GameType.ARMA2).Equals(String.Empty))
                        {
                            return Directory.GetFiles(GetBaseDirectory(GameType.ARMA2), "*.vdf", SearchOption.AllDirectories).Length != 0;
                        }
                        else
                        {
                            return false;
                        }

                    case GameType.ARMA2OA:
                    case GameType.ARMA2OABETA:
                        if (!GetBaseDirectory(GameType.ARMA2OA).Equals(String.Empty))
                        {
                            return Directory.GetFiles(GetBaseDirectory(GameType.ARMA2OA), "*.vdf", SearchOption.AllDirectories).Length != 0;
                        }
                        else
                        {
                            return false;
                        }

                    case GameType.ARMA3:
                        return true;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool PathsSet(GameType pGameType)
        {
            try
            {
                switch (pGameType)
                {
                    case GameType.ARMA2:
                        if (SteamVersion(GameType.ARMA2))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2AddonPath.Equals(String.Empty));
                        }
                        else
                        {
                            return (!A2Path.Equals(String.Empty));
                        }

                    case GameType.ARMA2OA:
                        if(SteamVersion(GameType.ARMA2OA))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2OAPath.Equals(String.Empty) && !A2OAAddonPath.Equals(String.Empty));
                        }
                        else
                        {
                            return (!A2OAPath.Equals(String.Empty) && !A2OAAddonPath.Equals(String.Empty));
                        }
                        
                    case GameType.ARMA2OABETA:
                        if (SteamVersion(GameType.ARMA2OA))
                        {
                            return (!A2Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A2OAPath.Equals(String.Empty) && !A2OAAddonPath.Equals(String.Empty) && !A2OABetaPath.Equals(String.Empty));
                        }
                        else
                        {
                            return (!A2OAPath.Equals(String.Empty) && !A2OAAddonPath.Equals(String.Empty));
                        }

                    case GameType.ARMA3:
                        if (SteamVersion(GameType.ARMA3))
                        {
                            return (!A3Path.Equals(String.Empty) && !SteamPath.Equals(String.Empty) && !A3AddonPath.Equals(String.Empty));
                        }
                        else
                        {
                            // Never gonna happen.
                            return (!A3Path.Equals(String.Empty) && !A3AddonPath.Equals(String.Empty));
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception)
            {
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
                Properties.Settings.Default.Steam_Path      = this.SteamPath;
                Properties.Settings.Default.A2_Path         = this.A2Path;
                Properties.Settings.Default.A2OA_Path       = this.A2OAPath;
                Properties.Settings.Default.A2OABeta_Path   = this.A2OABetaPath;
                Properties.Settings.Default.A3_Path         = this.A3Path;
                Properties.Settings.Default.A2OA_Addon_Path = this.A2OAAddonPath;
                Properties.Settings.Default.A3_Addon_Path   = this.A3AddonPath;

                Properties.Settings.Default.Save();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
