using ApplicationUpdate.Classes;
using ApplicationUpdate.Enums;
using ApplicationUpdate.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ApplicationUpdate
{
    class Program
    {
        /*
         * Purpose of this application is to update the main UpdateClient, when any changes to files are required.   
         */

        [STAThread]
        static void Main(string[] args)
        {
            /* Fields */
            ApplicationManifest RemoteManifest;
            ApplicationManifest LocalManifest;

            try
            {
                /* 
                 * The application should work in the following way, after launch it should connect to the server and receive a list of files and their version.
                 * It should then check the filesystem, for whether those files are present. If not - the files should be downloaded.
                 * After it has ensured that the files are present - it should check the local manifest and the remote manifest, and see if file versions are the same.
                 * If not - it should delete those files and re-download them.
                 * Optionally - it should check the hash for the file to guarantee cotinuity.
                 */

                Console.WriteLine("{0} [AppUpdate] -->> Updating application files. Please stand by...", DateTime.Now.ToString("HH:mm:ss"));

                // Due to the usage of relative paths, the working directory must be set to that of the application executable
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

                // Load the local manifest
                LocalManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(
                    Properties.Settings.Default.LocalManifest, typeof(ApplicationManifest));

                // Download the remote manifest
                RemoteManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(
                    NetworkUtilities.DownloadToString(Properties.Settings.Default.RemoteManifest).Result , typeof(ApplicationManifest));
               
                // Initiate the update process.
                foreach (ApplicationFile RemoteFile in RemoteManifest.ApplicationFileList)
                {
                    // Check whether the file is supposed to be updated by the updater.
                    if (RemoteFile.Type.Equals(FileType.UPDATER))
                    {
                        // Check if the file is present in the local manifest
                        if (LocalManifest.ApplicationFileList.Any(p => p.ID.Equals(RemoteFile.ID)))
                        {
                            ApplicationFile LocalFile = LocalManifest.ApplicationFileList.Find(p => p.ID.Equals(RemoteFile.ID));

                            // There is a record of such file in a local manifest.
                            if (File.Exists(RemoteFile.Path))
                            {
                                // Check to see whether the file version is the same in both manifests.
                                if (RemoteFile.Version.Equals(LocalFile.Version))
                                {
                                    // The versions are the same.
                                    // Welp, nothing to do here...
                                }
                                else
                                {
                                    // The versions are not the same.
                                    Console.WriteLine("{0} [AppUpdate] -->> Replacing file '{1}'", DateTime.Now.ToString("HH:mm:ss"), RemoteFile.Path);
                                    // Delete the local file.
                                    FileUtilities.DeleteFile(LocalFile.Path).Wait();
                                    // Download a new one in it's place.
                                    NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                                }
                            }
                            else
                            {
                                // No such file is present, even though the manifest has a record regarding it.
                                Console.WriteLine("{0} [AppUpdate] -->> Downloading file '{1}'", DateTime.Now.ToString("HH:mm:ss"), RemoteFile.Path);
                                NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                            }
                        }
                        else
                        {
                            // There is no such record in the local manifest
                            // So it has to be downloaded in any case.
                            Console.WriteLine("{0} [AppUpdate] -->> Downloading file '{1}'", DateTime.Now.ToString("HH:mm:ss"), RemoteFile.Path);
                            NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                        }
                    }
                }

                // Now check for the files that are present in the local manifest, but are not present in the remote manifest. 
                // Such files have to be deleted.
                foreach(ApplicationFile LocalFile in LocalManifest.ApplicationFileList)
                {
                    if(!RemoteManifest.ApplicationFileList.Any(p => p.ID.Equals(LocalFile.ID)))
                    {
                        // Remote manifest does not contain an entry with such and ID
                        if(File.Exists(LocalFile.Path))
                        {
                            Console.WriteLine("{0} [AppUpdate] -->> Deleting file '{1}'", DateTime.Now.ToString("HH:mm:ss"), LocalFile.Path);
                            FileUtilities.DeleteFile(LocalFile.Path).Wait();
                        }
                    }
                }

                // Save the remote manifest instead of a local one.
                File.WriteAllText(Properties.Settings.Default.LocalManifest, RemoteManifest.XmlSerializeToString());

                Console.WriteLine("{0} [AppUpdate] -->> Update complete.", DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Should be logged.
            }
            finally
            {
                Console.WriteLine("{0} [AppUpdate] -->> Press ENTER to continue.", DateTime.Now.ToString("HH:mm:ss"));
                Console.ReadLine();

                // Launch the updated application.
                new Process() { StartInfo = new ProcessStartInfo(Properties.Settings.Default.ApplicationPath, String.Empty) }.Start();
            }
        }
    }
}
