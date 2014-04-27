using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Client.Properties;
using log4net;

namespace Client.Core.Utilities
{
    public static class NetworkUtilities
    {
        // Logger
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Downloads a file from a remote host and downloads it to a local location.
        /// </summary>
        /// <param name="pInputUrl">URL of the file to be downloaded.</param>
        /// <param name="pOutputPath">Output for the result</param>
        /// <returns></returns>
        public static async Task<Boolean> DownloadToFile(String pInputUrl, String pOutputPath)
        {
            for (var cycleCount = 0; cycleCount < Settings.Default.RetryAttempts; cycleCount++)
            {
                try
                {
                    //await new WebClient().DownloadFileTaskAsync(pInputUrl, pOutputPath);
                    new WebClient().DownloadFile(pInputUrl, pOutputPath);
                    return true;
                }
                catch (WebException ex)
                {
                    // File not accessable or timed out while trying to download. In any case - do nothing.
                    Logger.Error("An error was encountered while trying to download a file.", ex);
                }
                catch (Exception ex)
                {
                    Logger.Error("An error was encountered while trying to download a file.", ex);
                }
            }
            return false;
        }

        /// <summary>
        /// Downloads a file from a remote host and returns it's contents as a string.
        /// </summary>
        /// <param name="pInputUrl">File to be downloaded</param>
        /// <returns>File content string</returns>
        public static async Task<String> DownloadToString(String pInputUrl)
        {
            return await new WebClient().DownloadStringTaskAsync(pInputUrl);
        }

        public static async Task<String> GetMirror(IEnumerable<String> pMirrorList)
        {
            try
            {
                var availableMirrors = new List<String>();
                var randomGenerator = new Random();

                foreach (var mirror in pMirrorList)
                {
                    try
                    {
                        var mirrorUri = new Uri(mirror);
                        if (mirrorUri.Scheme.Equals("ftp"))
                        {
                            if (GetFtpMirror(mirrorUri).Result)
                            {
                                availableMirrors.Add(mirrorUri.ToString());
                            }
                        }
                        else if (mirrorUri.Scheme.Equals("http"))
                        {
                            if (GetHttpMirror(mirrorUri).Result)
                            {
                                availableMirrors.Add(mirrorUri.ToString());
                            }
                        }
                        else
                        {
                            throw new ApplicationException("Unknown URI schema");
                        }
                    }
                    catch (WebException ex)
                    {
                        Logger.Error("An error was encountered while awaiting for a response from a mirror.", ex);
                    }
                }

                if (availableMirrors.Count > 0)
                {
                    return availableMirrors[randomGenerator.Next(0, availableMirrors.Count)];
                }
                throw new ApplicationException("No mirrors available");
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to find a mirror.", ex);
                throw;
            }
        }

        private static async Task<Boolean> GetFtpMirror(Uri pFtpMirror)
        {
            FtpWebResponse checkMirrorResponse = null;
            try
            {
                var checkMirrorRequest = (FtpWebRequest) WebRequest.Create(pFtpMirror);
                checkMirrorRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                checkMirrorRequest.Timeout = 45000;
                checkMirrorRequest.UseBinary = true;
                checkMirrorResponse = (FtpWebResponse) checkMirrorRequest.GetResponse();

                return (checkMirrorResponse.StatusCode == FtpStatusCode.FileStatus);
            }
            catch (WebException ex)
            {
                Logger.Error("An error was encountered while awaiting for a response from an FTP mirror.", ex);
                return false;
            }
            finally
            {
                if (checkMirrorResponse != null)
                {
                    checkMirrorResponse.Close();
                }
            }
        }

        private static async Task<Boolean> GetHttpMirror(Uri pFtpMirror)
        {
            HttpWebResponse checkMirrorResponse = null;
            try
            {
                var checkMirrorRequest = (HttpWebRequest) WebRequest.Create(pFtpMirror);
                checkMirrorResponse = (HttpWebResponse) checkMirrorRequest.GetResponse();

                return (checkMirrorResponse.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while awaiting for a response from an HTTP mirror.", ex);
                return false;
            }
            finally
            {
                if (checkMirrorResponse != null)
                    checkMirrorResponse.Close();
            }
        }
    }
}
