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
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
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
            catch(OperationCanceledException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
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
                var availableMirrors = new List<String>();
                var randomGenerator = new Random();

                foreach (var ftpMirror in pFtpMirrorList)
                {
                    var checkMirrorRequest = (FtpWebRequest)WebRequest.Create(ftpMirror);
                    checkMirrorRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    var checkMirrorResponse = (FtpWebResponse)checkMirrorRequest.GetResponse();

                    if (checkMirrorResponse.StatusCode == FtpStatusCode.FileStatus)
                    {
                        availableMirrors.Add(ftpMirror);
                    }

                    checkMirrorResponse.Close();
                }

                if (availableMirrors.Count > 0)
                {
                    return availableMirrors[randomGenerator.Next(0, availableMirrors.Count)];
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
                var availableMirrors = new List<String>();
                var randomGenerator = new Random();

                foreach (var httpMirror in pHttpMirrorList)
                {
                    var checkMirrorRequest = (HttpWebRequest)WebRequest.Create(httpMirror);
                    var checkMirrorResponse = (HttpWebResponse)checkMirrorRequest.GetResponse();

                    if (checkMirrorResponse.StatusCode == HttpStatusCode.OK)
                    {
                        availableMirrors.Add(httpMirror);
                    }

                    checkMirrorResponse.Close();
                }

                if (availableMirrors.Count > 0)
                {
                    return availableMirrors[randomGenerator.Next(0, availableMirrors.Count)];
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
