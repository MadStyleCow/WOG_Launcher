using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Client.Core.Utilities
{
    public static class NetworkUtilities
    {
        /// <summary>
        /// Downloads a file from a remote host and downloads it to a local location.
        /// </summary>
        /// <param name="pInputUrl">URL of the file to be downloaded.</param>
        /// <param name="pOutputPath">Output for the result</param>
        /// <returns></returns>
        public static async Task<Boolean> DownloadToFile(String pInputUrl, String pOutputPath)
        {
            for (int CycleCount = 0; CycleCount < Properties.Settings.Default.RetryAttempts; CycleCount++)
            {
                try
                {
                    await new WebClient().DownloadFileTaskAsync(pInputUrl, pOutputPath);
                    return true;
                }
                catch(OperationCanceledException)
                {
                    throw;
                }
                catch(WebException)
                {
                    // File not accessable or timed out while trying to download. In any case - do nothing.
                }
                catch (Exception)
                {
                    throw;
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
                List<String> AvailableMirrors = new List<String>();
                Random RandomGenerator = new Random();

                foreach (String FtpMirror in pFtpMirrorList)
                {
                    FtpWebRequest CheckMirrorRequest = (FtpWebRequest)WebRequest.Create(FtpMirror);
                    CheckMirrorRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    FtpWebResponse CheckMirrorResponse = (FtpWebResponse)CheckMirrorRequest.GetResponse();

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
                else
                {
                    throw new ApplicationException("No mirrors available.");
                }
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
        /// <param name="pHttpMirrorList">List of mirrors to be tested.</param>
        /// <returns>A randomly selected, working mirror.</returns>
        public static async Task<String> GetHttpMirror(List<String> pHttpMirrorList)
        {
            try
            {
                List<String> AvailableMirrors = new List<String>();
                Random RandomGenerator = new Random();

                foreach (String HttpMirror in pHttpMirrorList)
                {
                    HttpWebRequest CheckMirrorRequest = (HttpWebRequest)WebRequest.Create(HttpMirror);
                    HttpWebResponse CheckMirrorResponse = (HttpWebResponse)CheckMirrorRequest.GetResponse();

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
                else
                {
                    throw new ApplicationException("No mirrors available.");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
