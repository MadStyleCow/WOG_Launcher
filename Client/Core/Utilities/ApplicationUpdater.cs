using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Core.Utilities.Classes;
using Client.Core.Enums;
using System.IO;

namespace Client.Core.Utilities
{
    public static class ApplicationUpdater
    {
        /* Loggers */
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(ApplicationUpdater));

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
                Task<ApplicationManifest> AppManifestTask = NetworkUtilities.DownloadToString(pRemoteManifest).ContinueWith<ApplicationManifest>(t => 
                    (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(t.Result, typeof(ApplicationManifest)));

                // Load the local manifest
                ApplicationManifest LocalManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(pLocalManifest, typeof(ApplicationManifest));

                return (await AppManifestTask).ManifestVersion.Equals(LocalManifest.ManifestVersion);
            }
            catch(Exception)
            {             
                throw;
            }
        }

        public static async Task Update(String pRemoteManifest, String pLocalManifest)
        {
            try
            {
                // Preload the app manifest
                Task<ApplicationManifest> AppManifestTask = NetworkUtilities.DownloadToString(pRemoteManifest).ContinueWith<ApplicationManifest>(t =>
                    (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(t.Result, typeof(ApplicationManifest)));

                // Load the local manifest
                ApplicationManifest LocalManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(pLocalManifest, typeof(ApplicationManifest));
                ApplicationManifest RemoteManifest = await AppManifestTask;

                foreach (ApplicationFile RemoteFile in RemoteManifest.ApplicationFileList.FindAll(p => p.Type.Equals(FileType.UPDATER)))
                {
                    if (LocalManifest.ApplicationFileList.Any(p => p.ID.Equals(RemoteFile.ID)))
                    {
                        ApplicationFile LocalFile = LocalManifest.ApplicationFileList.Find(p => p.ID.Equals(RemoteFile.ID));

                        if (File.Exists(RemoteFile.Path))
                        {
                            if (!RemoteFile.Version.Equals(LocalFile.Version))
                            {
                                await FileUtilities.DeleteFile(LocalFile.Path);
                                await NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path);
                            }
                        }
                        else
                        {
                            await NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path);
                        }
                    }
                    else
                    {
                        await NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path);
                    }
                }


            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
