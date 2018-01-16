using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Client.Core.Enums;
using Client.Core.Utilities.Classes;
using log4net;

namespace Client.Core.Utilities
{
    public static class ApplicationUpdater
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ApplicationUpdater));

        /// <summary>
        /// Indicates whether the application requires an update.
        /// </summary>
        /// <param name="pRemoteManifest">Remote application manifest URI.</param>
        /// <param name="pLocalManifest">Local application manifest URI.</param>
        /// <returns></returns>
        public static async Task<Boolean> UpdatesRequired(String pRemoteManifest, String pLocalManifest)
        {
            try
            {
                // Get the app manifest
                var appManifestTask = NetworkUtilities.DownloadToString(pRemoteManifest).ContinueWith(t => 
                    (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(t.Result, typeof(ApplicationManifest)));

                // Load the local manifest
                var localManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(pLocalManifest, typeof(ApplicationManifest));

                return (!(await appManifestTask).ManifestVersion.Equals(localManifest.ManifestVersion));
            }
            catch(Exception ex)
            {
                Logger.Error("An exception occured while trying to check the remote application manifest.", ex);
                return false;
            }
        }

        public static async Task Update(String pRemoteManifest, String pLocalManifest)
        {
            try
            {
                // Preload the app manifest
                var appManifestTask = NetworkUtilities.DownloadToString(pRemoteManifest).ContinueWith(t =>
                    (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(t.Result, typeof(ApplicationManifest)));

                // Load the local manifest
                var localManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(pLocalManifest, typeof(ApplicationManifest));
                var remoteManifest = await appManifestTask;

                foreach (var remoteFile in remoteManifest.ApplicationFileList.FindAll(p => p.Type.Equals(FileType.Updater)))
                {
                    // Create a full path for the file
                    string _globalFilePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + remoteFile.Path);

                    if (localManifest.ApplicationFileList.Any(p => p.Id.Equals(remoteFile.Id)))
                    {
                        var localFile = localManifest.ApplicationFileList.Find(p => p.Id.Equals(remoteFile.Id));

                        if (File.Exists(_globalFilePath))
                        {
                            if (!remoteFile.Version.Equals(localFile.Version))
                            {
                                FileUtilities.DeleteFile(_globalFilePath);
                                NetworkUtilities.DownloadToFile(NetworkUtilities.GetMirror(remoteFile.Url), _globalFilePath);
                            }
                        }
                        else
                        {
                            // This works with relative paths, however in order to check the file-size
                            // We need to have the full file file path
                            NetworkUtilities.DownloadToFile(NetworkUtilities.GetMirror(remoteFile.Url), _globalFilePath);
                        }
                    }
                    else
                    {
                        // This works with relative paths, however in order to check the file-size
                        // We need to have the full file file path
                        NetworkUtilities.DownloadToFile(NetworkUtilities.GetMirror(remoteFile.Url), _globalFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to update the application.", ex);
                throw ex;
            }
        }
    }
}
