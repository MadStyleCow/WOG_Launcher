using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Client.Core.Classes;
using Client.Core.Enums;
using Client.Core.Utilities.Classes;

namespace Client.Core.Utilities
{
    public static class GameLauncher
    {
        /// <summary>
        /// Launches the game with the desired settings.
        /// </summary>
        /// <param name="pServer">Selected server data.</param>
        /// <param name="pSettings">Selected launch settings.</param>
        /// <exception cref="NotImplementedException"></exception>
        public static bool LaunchGame(Server pServer, SettingsCacheEntry pSettings)
        {
            try
            {
                Process game = new Process();
                String argumentString = GenerateParameterString(pServer, pSettings);

                switch (pServer.Type)
                {
                    #region ARMA2
                    case GameType.Arma2:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2))
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                  FileName = LocalMachine.Instance.GetExecutable(GameType.Steam),
                                  Arguments = argumentString
                            };  
                        }
                        else
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Arma2),
                                Arguments = argumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA2OA
                    case GameType.Arma2Oa:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2Oa))
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Steam),
                                Arguments = argumentString
                            };
                        }
                        else
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Arma2Oa),
                                Arguments = argumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA2OABeta
                    case GameType.Arma2Oabeta:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2Oa))
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Steam),
                                Arguments = argumentString
                            };
                        }
                        else
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Arma2Oabeta),
                                WorkingDirectory = LocalMachine.Instance.GetBaseDirectory(GameType.Arma2Oa),
                                Arguments = argumentString
                            };
                        }
                        break;
                    #endregion

                    #region ARMA3
                    case GameType.Arma3:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma3))
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Steam),
                                Arguments = argumentString
                            };
                        }
                        else
                        {
                            game.StartInfo = new ProcessStartInfo
                            {
                                FileName = LocalMachine.Instance.GetExecutable(GameType.Arma3),
                                Arguments = argumentString
                            };
                        }
                        break;
                    #endregion

                    default:
                        throw new NotImplementedException();
                }

                return game.Start();
            }
            catch(Exception ex)
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
        private static string GenerateParameterString(Server pServer, SettingsCacheEntry pSettings)
        {
            try
            {
                StringBuilder parameterString = new StringBuilder();

                switch (pServer.Type)
                {
                    #region ARMA2
                    case GameType.Arma2:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2))
                            parameterString.Append("-applaunch 33900 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            parameterString.Append("\"-mod=");
                            foreach (var modEntry in pSettings.ModList)
                            {
                                parameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.Arma2), modEntry));
                            }
                            parameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA2OA
                    case GameType.Arma2Oa:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2Oa))
                            parameterString.Append("-applaunch 33930 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            parameterString.AppendFormat("\"-mod={0};{1};", LocalMachine.Instance.GetBaseDirectory(GameType.Arma2), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.Arma2Oa), "expansion"));
                            foreach (var modEntry in pSettings.ModList)
                            {
                                parameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.Arma2Oa), modEntry));
                            }
                            parameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA2OABeta
                    case GameType.Arma2Oabeta:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma2Oa))
                            parameterString.Append("-applaunch 219540");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            parameterString.AppendFormat(" \"-beta={0};{1};\" ", LocalMachine.Instance.GetBaseDirectory(GameType.Arma2Oabeta), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.Arma2Oabeta), "expansion"));

                            if (LocalMachine.Instance.SteamVersion(GameType.Arma2Oa))
                            {
                                parameterString.AppendFormat("\"-mod={0};{1};", LocalMachine.Instance.GetBaseDirectory(GameType.Arma2), Path.Combine(LocalMachine.Instance.GetBaseDirectory(GameType.Arma2Oa), "expansion"));
                            }
                            else
                            {
                                parameterString.Append("\"-mod=");
                            }

                            foreach (var modEntry in pSettings.ModList)
                            {
                                parameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.Arma2Oa), modEntry));
                            }
                            parameterString.Append("\" ");
                        }
                        break;
                    #endregion

                    #region ARMA3
                    case GameType.Arma3:
                        if (LocalMachine.Instance.SteamVersion(GameType.Arma3))
                            parameterString.Append("-applaunch 107410 ");

                        // Add mods
                        if (pSettings.ModList.Count != 0)
                        {
                            parameterString.Append("\"-mod=");
                            foreach (var modEntry in pSettings.ModList)
                            {
                                parameterString.AppendFormat("{0};", Path.Combine(LocalMachine.Instance.GetModDirectory(GameType.Arma3), modEntry));
                            }
                            parameterString.Append("\" ");
                        }
                        break;
                    #endregion
                }

                #region Common Parameter Block
                if (pSettings.NoSplash)
                    parameterString.Append("-nosplash ");

                if (pSettings.ShowScriptErrors)
                    parameterString.Append("-showScriptErrors ");

                if (pSettings.EmptyWorld)
                    parameterString.Append("-world=empty ");

                if (pSettings.Windowed)
                    parameterString.Append("-window ");

                if (pSettings.WinXp)
                    parameterString.Append("-winxp ");

                if (pSettings.CpuCountChecked)
                    parameterString.AppendFormat("-cpuCount={0} ", pSettings.CpuCount);

                if (pSettings.ExThreadsChecked)
                    parameterString.AppendFormat("-exThreads={0} ", pSettings.ExThreads);

                if (pSettings.MaxMemoryChecked)
                    parameterString.AppendFormat("-maxMem={0} ", pSettings.MaxMemory);

                if (pSettings.AutoConnect)
                    parameterString.AppendFormat("-connect={0} -port={1} -password={2} ", pServer.Hostname, pServer.Port, pServer.Password);

                if (pSettings.AdditionalParameters != String.Empty)
                    parameterString.Append(pSettings.AdditionalParameters);
                #endregion

                return parameterString.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
