using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateClient.Core.Enums;

namespace UpdateClient.Core.Utilities
{
    public class LocalMachine
    {
        /* Static fields */
        public static LocalMachine Instance = new LocalMachine();

        /* Private fields */
        public OperatingSystem OS { get; private set; }
        public Int32 CpuCount { get; private set; }
        public String A2Path { get; private set; }
        public String A2OAPath { get; private set; }
        public String A2OABetaPath { get; private set; }
        public String A3Path { get; private set; }
        public String SteamPath { get; private set; }

        /* Constructors */
        public LocalMachine()
        {
            // Assign local machine parameters
            this.OS = Environment.OSVersion;
            this.CpuCount = Environment.ProcessorCount;

            // Assign paths
            this.SteamPath = LocatePath(GameType.STEAM);
            this.A2Path = LocatePath(GameType.ARMA2);
            this.A2OAPath = LocatePath(GameType.ARMA2OA);
            this.A2OABetaPath = LocatePath(GameType.ARMA2OABETA);
            this.A3Path = LocatePath(GameType.ARMA3);
        }

        /* Private methods */
        private string LocatePath(GameType pGameType)
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
                            // Try to locate the steam path based on the registry settings.
                            throw new NotImplementedException();
                        }

                    case GameType.ARMA2:
                        if(!Properties.Settings.Default.A2_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2_Path;
                        }
                        else
                        {
                            // Try to locate the steam path based on the registry settings.
                            throw new NotImplementedException();
                        }

                    case GameType.ARMA2OA:
                        if(!Properties.Settings.Default.A2OA_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2OA_Path;
                        }
                        else
                        {
                            // Try to locate the steam path based on the registry settings.
                            throw new NotImplementedException();
                        }

                    case GameType.ARMA2OABETA:
                        if(!Properties.Settings.Default.A2OABeta_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A2OABeta_Path;
                        }
                        else
                        {
                            // Try to locate the steam path based on the registry settings.
                            // As a fallback - try to use the ARMA2OA path with an added /expansion/beta/arma2oa.exe
                            throw new NotImplementedException();
                        }

                    case GameType.ARMA3:
                        if(!Properties.Settings.Default.A3_Path.ToLowerInvariant().Equals(String.Empty))
                        {
                            return Properties.Settings.Default.A3_Path;
                        }
                        else
                        {
                            // Try to locate the steam path based on the registry settings.
                            throw new NotImplementedException();
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
        public string GetPath(GameType pGameType)
        {
            try
            {
                switch(pGameType)
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
            catch(Exception)
            {
                throw;
            }
        }

        public void Save()
        {
            try
            {
                Properties.Settings.Default.Steam_Path      = this.SteamPath;
                Properties.Settings.Default.A2_Path         = this.A2Path;
                Properties.Settings.Default.A2OA_Path       = this.A2OAPath;
                Properties.Settings.Default.A2OABeta_Path   = this.A2OABetaPath;
                Properties.Settings.Default.A3_Path         = this.A3Path;

                Properties.Settings.Default.Save();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
