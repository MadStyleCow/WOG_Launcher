using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Updater.Properties;

namespace Updater.Utilities
{
    public static class NetworkUtilities
    {
        /// <summary>
        /// Downloads a file from a remote host and downloads it to a local location.
        /// </summary>
        /// <param name="pInputUrl">URL of the file to be downloaded.</param>
        /// <param name="pOutputPath">Output for the result</param>
        /// <returns></returns>
        public static async Task DownloadToFile(String pInputUrl, String pOutputPath)
        {
            for (var CycleCount = 0; CycleCount < Settings.Default.RetryAttempts; CycleCount++)
            {
                try
                {
                    await new WebClient().DownloadFileTaskAsync(pInputUrl, pOutputPath);
                }
                catch(OperationCanceledException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Downloads a file from a remote host and returns it's contents as a string.
        /// </summary>
        /// <param name="pInputUrl">File to be downloaded</param>
        /// <returns>File content string</returns>
        public static async Task<String> DownloadToString(String pInputUrl)
        {
            try
            {
                return await new WebClient().DownloadStringTaskAsync(pInputUrl);
            }
            catch(OperationCanceledException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Tests the list of mirrors for working ones.
        /// </summary>
        /// <param name="pFtpMirrorList">List of mirrors to be tested.</param>
        /// <returns>A randomly selected, working mirror.</returns>
        public static async Task<String> GetFtpMirror(List<String> pFtpMirrorList)
        {
            try
            {
                var AvailableMirrors = new List<String>();
                var RandomGenerator = new Random();

                foreach (var FtpMirror in pFtpMirrorList)
                {
                    var CheckMirrorRequest = (FtpWebRequest)WebRequest.Create(FtpMirror);
                    CheckMirrorRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    var CheckMirrorResponse = (FtpWebResponse)CheckMirrorRequest.GetResponse();

                    if (CheckMirrorResponse.StatusCode == FtpStatusCode.FileStatus)
                    {
                        AvailableMirrors.Add(FtpMirror);
                    }

                    CheckMirrorResponse.Close();
                }

                if (AvailableMirrors.Count > 0)
                {
                    return AvailableMirrors[RandomGenerator.Next(0, AvailableMirrors.Count)];
                }
                throw new ApplicationException("No mirrors available.");
            }
            catch(OperationCanceledException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Tests the list of mirrors for working ones.
        /// </summary>
        /// <param name="pHttpMirrorList">List of mirrors to be tested.</param>
        /// <returns>A randomly selected, working mirror.</returns>
        public static async Task<String> GetHttpMirror(List<String> pHttpMirrorList)
        {
            try
            {
                var AvailableMirrors = new List<String>();
                var RandomGenerator = new Random();

                foreach (var HttpMirror in pHttpMirrorList)
                {
                    var CheckMirrorRequest = (HttpWebRequest)WebRequest.Create(HttpMirror);
                    var CheckMirrorResponse = (HttpWebResponse)CheckMirrorRequest.GetResponse();

                    if (CheckMirrorResponse.StatusCode == HttpStatusCode.OK)
                    {
                        AvailableMirrors.Add(HttpMirror);
                    }

                    CheckMirrorResponse.Close();
                }

                if (AvailableMirrors.Count > 0)
                {
                    return AvailableMirrors[RandomGenerator.Next(0, AvailableMirrors.Count)];
                }
                throw new ApplicationException("No mirrors available.");
            }
            catch (OperationCanceledException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
