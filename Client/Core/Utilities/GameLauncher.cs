using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Core.Classes;
using Client.Core.Utilities.Classes;
using Client.Core.Enums;
using System.Diagnostics;
using System.IO;

namespace Client.Core.Utilities
{
    public static class GameLauncher
    {
        /// <summary>
        /// Launches the game with the desired settings.
        /// </summary>
        /// <param name="pServer">Selected server data.</param>
        /// <param name="pSettings">Selected launch settings.</param>
        public static bool LaunchGame(Server pServer, SettingsCacheEntry pSettings)
        {
            try
            {
                Process Game = new Process();
                String ArgumentString = GenerateParameterString(pServer, pSettings);

                switch (pServer.Type)
                {
                    #region ARMA2
                    case GameType.ARMA2:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2))
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                  FileName = LocalMachine.Instance.SteamPath,
                                  Arguments = ArgumentString
                            };  
                        }
                        else
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.A2Path,
                                Arguments = ArgumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA2OA
                    case GameType.ARMA2OA:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2OA))
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.SteamPath,
                                Arguments = ArgumentString
                            };
                        }
                        else
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.A2OAPath,
                                Arguments = ArgumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA2OABeta
                    case GameType.ARMA2OABETA:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2OA))
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.SteamPath,
                                Arguments = ArgumentString
                            };
                        }
                        else
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.A2OABetaPath,
                                WorkingDirectory = LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2OA),
                                Arguments = ArgumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA3
                    case GameType.ARMA3:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA3))
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.SteamPath,
                                Arguments = ArgumentString
                            };
                        }
                        else
                        {
                            Game.StartInfo = new ProcessStartInfo()
                            {
                                FileName = LocalMachine.Instance.A3Path,
                                Arguments = ArgumentString
                            };
                        }
                        break;
                    #endregion

                    default:
                        throw new NotImplementedException();
                }

                return Game.Start();
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generates a parameter string for the selected game \ version combination.
        /// </summary>
        /// <param name="pServer">Server for which the string should be generated.</param>
        /// <param name="pSettings">Settings for which the settings should be generated.</param>
        /// <returns>Parameter string</returns>
        public static string GenerateParameterString(Server pServer, SettingsCacheEntry pSettings)
        {
            try
            {
                StringBuilder ParameterString = new StringBuilder();

                switch (pServer.Type)
                {
                    #region ARMA2
                    case GameType.ARMA2:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2))
                            ParameterString.Append("-applaunch 33900 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            ParameterString.Append("\"-mod=");
                            foreach (String ModEntry in pSettings.ModList)
                            {
                                ParameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.ARMA2), ModEntry));
                            }
                            ParameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA2OA
                    case GameType.ARMA2OA:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2OA))
                            ParameterString.Append("-applaunch 33930 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            ParameterString.AppendFormat("\"-mod={0};{1};", LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2OA), "expansion"));
                            foreach (String ModEntry in pSettings.ModList)
                            {
                                ParameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.ARMA2OA), ModEntry));
                            }
                            ParameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA2OABeta
                    case GameType.ARMA2OABETA:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA2OA))
                            ParameterString.Append("-applaunch 219540 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            ParameterString.AppendFormat("\"-beta={0};{1};\" ", LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2OABETA), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2OABETA), "expansion"));

                            if (LocalMachine.Instance.SteamVersion(GameType.ARMA2OA))
                            {
                                ParameterString.AppendFormat("\"-mod={0};{1};", LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.ARMA2OA), "expansion"));
                            }
                            else
                            {
                                ParameterString.Append("\"-mod=");
                            }

                            foreach (String ModEntry in pSettings.ModList)
                            {
                                ParameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.ARMA2OA), ModEntry));
                            }
                            ParameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA3
                    case GameType.ARMA3:
                        if (LocalMachine.Instance.SteamVersion(GameType.ARMA3))
                            ParameterString.Append("-applaunch 107410 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            ParameterString.Append("\"-mod=");
                            foreach (String ModEntry in pSettings.ModList)
                            {
                                ParameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.ARMA3), ModEntry));
                            }
                            ParameterString.Append("\" ");
                        }
                        break;
                    #endregion
                }

                #region Common Parameter Block
                if (pSettings.NoSplash)
                    ParameterString.Append("-nosplash ");

                if (pSettings.ShowScriptErrors)
                    ParameterString.Append("-showScriptErrors ");

                if (pSettings.EmptyWorld)
                    ParameterString.Append("-world=empty ");

                if (pSettings.Windowed)
                    ParameterString.Append("-window ");

                if (pSettings.WinXP)
                    ParameterString.Append("-winxp ");

                if (pSettings.CpuCountChecked)
                    ParameterString.AppendFormat("-cpuCount={0} ", pSettings.CpuCount);

                if (pSettings.ExThreadsChecked)
                    ParameterString.AppendFormat("-exThreads={0} ", pSettings.ExThreads);

                if (pSettings.MaxMemoryChecked)
                    ParameterString.AppendFormat("-maxMem={0} ", pSettings.MaxMemory);

                if (pSettings.AutoConnect)
                    ParameterString.AppendFormat("-connect={0} -port={1} -password={2} ", pServer.Hostname, pServer.Port, pServer.Password);

                if (pSettings.AdditionalParameters != String.Empty)
                    ParameterString.Append(pSettings.AdditionalParameters);
                #endregion

                return ParameterString.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
