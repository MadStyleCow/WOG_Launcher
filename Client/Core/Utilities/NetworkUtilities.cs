using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Client.Properties;
using log4net;

namespace Client.Core.Utilities
{
    public static class NetworkUtilities
    {
        // Logger
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Public methods
        /// <summary>
        /// This method is used to download a file via FTP and save it to disk.
        /// </summary>
        /// <param name="pInputUrl">URL of the file to be downloaded.</param>
        /// <param name="pOutputPath">URI of where to save the file.</param>
        /// <returns></returns>
        public static bool DownloadToFile(String pInputUrl, String pOutputPath)
        {
            // Define the count of retries
            int _retryCount = Settings.Default.RetryAttempts;

            // Try downloading the file
            try
            {
                // But first - check if there is enough free space available in the temp
                DriveInfo _tempDrive = new DriveInfo(pOutputPath.Substring(0, 1));

                // Request the size of the file
                FtpWebRequest _sizeRequest = (FtpWebRequest)WebRequest.Create(pInputUrl);

                // Set params
                _sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                _sizeRequest.Timeout = 10000;
                _sizeRequest.UseBinary = true;

                // Execute
                FtpWebResponse _sizeResponse = (FtpWebResponse)_sizeRequest.GetResponse();

                // Is it enough?
                if (_sizeResponse.StatusCode == FtpStatusCode.FileStatus)
                {
                    if (_sizeResponse.ContentLength > _tempDrive.TotalFreeSpace)
                    {
                        throw new IOException("Not enough free space in TEMP folder.");
                    }
                }

                // Close the response
                _sizeResponse.Close();

                // And then simply use a web-client to download the file
                // With several retries
                for (var i = 0; i < _retryCount; i++)
                {
                    // Try to download the file
                    new WebClient().DownloadFile(pInputUrl, pOutputPath);
                    return true;
                }

                // And if we are still here - return false
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method downloads the file as a string
        /// </summary>
        /// <param name="pInputUrl">URL of the file to be downloaded</param>
        /// <returns>Contents of the file as a string</returns>
        public static async Task<String> DownloadToString(String pInputUrl)
        {
            try
            {
                // Try to return the file content
                return new WebClient().DownloadString(pInputUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method tests the provided list of mirrors and selects an active one.
        /// </summary>
        /// <param name="pMirrorList">List of mirrors to be tested</param>
        /// <returns>An URL for the active mirror</returns>
        public static string GetMirror(List<String> pMirrorList)
        {
            try
            {
                // Create a new dictionary
                List<Task<bool>> _taskList = new List<Task<bool>>();

                // Test each mirror
                foreach (string _mirrorUrl in pMirrorList)
                {
                    // Create an URI
                    Uri _mirrorUri = new Uri(_mirrorUrl);

                    // Start a task and add it to an array
                    var _task = Task<bool>.Run(
                        () =>
                        {
                        // Test each mirror in the list
                        return TestMirror(_mirrorUri);
                        });

                    // Add it to the task list
                    _taskList.Add(_task);
                }

                // Due to the timeout on the FTP / HTTP requests - what we should do
                // Is return the first URI to return a true answer. It will most likely be the fastest one anyways.
                // And the amounts of traffic are quite small, so no need to worry regarding server overloads.
                // Wait for any of the tasks to complete
                var _taskIndex = Task.WaitAny(_taskList.ToArray(), 20000);

                // Return the value from the initial list, matching the index of the completed task.
                if (_taskIndex != -1)
                {
                    return pMirrorList[_taskIndex];
                }
                else
                {
                    throw new ApplicationException("No mirrors available");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Private methods
        /// <summary>
        /// This method tests the provided mirror (HTTP & FTP mirrors are supported) and checks whether those mirrors are available.
        /// </summary>
        /// <param name="pMirrorUri">Uri of the mirror to test</param>
        /// <returns>A boolean value, indicating whether the mirror is available.</returns>
        private static bool TestMirror(Uri pMirrorUri)
        {
            // Determine, what kind of an URI this is
            // Is it an HTTP uri?
            if (pMirrorUri.Scheme.Equals("http"))
            {
                // Yes, its an HTTP uri
                try
                {
                    // Create an HTTP request
                    HttpWebRequest _httpRequest = WebRequest.CreateHttp(pMirrorUri);

                    // Set request properties
                    _httpRequest.Timeout = 10000;

                    // Get answer
                    HttpWebResponse _httpResponse = (HttpWebResponse) _httpRequest.GetResponse();

                    // Was it a success?
                    bool _mirrorAvailable = (_httpResponse.StatusCode == HttpStatusCode.OK);

                    // Close the response
                    _httpResponse.Close();

                    // Return result
                    return _mirrorAvailable;
                }
                catch(Exception)
                {
                    return false;
                }
            }
            else if (pMirrorUri.Scheme.Equals("ftp"))
            {
                // It's an FTP uri
                try
                {
                    // Create and configure an FTP request
                    FtpWebRequest _ftpRequest = (FtpWebRequest)WebRequest.Create(pMirrorUri);

                    // Set properties
                    _ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    _ftpRequest.Timeout = 10000;
                    _ftpRequest.UseBinary = true;

                    // Create and read a response
                    FtpWebResponse _ftpResponse = (FtpWebResponse)_ftpRequest.GetResponse();

                    // Was it a success?
                    bool _mirrorAvailable = (_ftpResponse.StatusCode == FtpStatusCode.FileStatus);

                    // Close the response
                    _ftpResponse.Close();

                    // Return result
                    return _mirrorAvailable;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
