using Updater.Classes;
using Updater.Enums;
using Updater.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using log4net.Config;

namespace Updater
{
    class Program
    {
        /*
         * Purpose of this application is to update the main UpdateClient, when any changes to files are required.   
         */
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(Program)); //log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        static void Main(string[] args)
        {
            // Configure the logger
            log4net.Config.XmlConfigurator.Configure();

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

                Logger.Info(String.Format("Application launched. OS information {0}",Environment.OSVersion.VersionString));

                // Wait for the application to shutdown.
                while(Process.GetProcesses().Any(p => p.StartInfo.FileName.Equals(Properties.Settings.Default.UpdateClientProcessName)))
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("{0} [AppUpdate] -->> Waiting for the update client to shut down...", DateTime.Now.ToString("HH:mm:ss"));
                    Logger.Info("Awaiting client shutdown.");
                }

                Console.WriteLine("{0} [AppUpdate] -->> Updating application files. Please stand by...", DateTime.Now.ToString("HH:mm:ss"));

                // Due to the usage of relative paths, the working directory must be set to that of the application executable
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

                // Load the local manifest
                LocalManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromFile(
                    Properties.Settings.Default.LocalManifest, typeof(ApplicationManifest));

                Logger.Info("Loaded the local manifest");

                // Download the remote manifest
                RemoteManifest = (ApplicationManifest)XMLSerializer.XmlDeserializeFromString(
                    NetworkUtilities.DownloadToString(Properties.Settings.Default.RemoteManifest).Result , typeof(ApplicationManifest));

                Logger.Info("Loaded the remote manifest");

                // Initiate the update process.
                foreach (ApplicationFile RemoteFile in RemoteManifest.ApplicationFileList.FindAll(p => p.Type.Equals(FileType.APPLICATION)))
                {
                    Logger.Info(String.Format("Processing file {0}", RemoteFile));

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
                                    Logger.Info(String.Format("{0} is up-to-date.", RemoteFile.Path));
                                }
                                else
                                {
                                    // The versions are not the same.
                                    Console.WriteLine("{0} [AppUpdate] -->> Replacing file '{1}'", DateTime.Now.ToString("HH:mm:ss"), RemoteFile.Path);
                                    Logger.Info(String.Format("{0} is out of date and requires an upate", RemoteFile.Path));
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
                                Logger.Info(String.Format("{0} is not present on the local machine and will be downloaded.", RemoteFile.Path));
                                NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
                            }
                        }
                        else
                        {
                            // There is no such record in the local manifest
                            // So it has to be downloaded in any case.
                            Console.WriteLine("{0} [AppUpdate] -->> Downloading file '{1}'", DateTime.Now.ToString("HH:mm:ss"), RemoteFile.Path);
                            Logger.Info(String.Format("{0} is not present in the local manifest and will be downloaded.", RemoteFile.Path));
                            NetworkUtilities.DownloadToFile(NetworkUtilities.GetFtpMirror(RemoteFile.URL).Result, RemoteFile.Path).Wait();
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
                            Logger.Info(String.Format("Deleting {0}", LocalFile.Path));
                            FileUtilities.DeleteFile(LocalFile.Path).Wait();
                        }
                    }
                }

                Console.WriteLine("{0} [AppUpdate] -->> Update complete.", DateTime.Now.ToString("HH:mm:ss"));
                Logger.Info("Update complete.");

                // Launch the updated application.
                 //new Process() { StartInfo = new ProcessStartInfo(Properties.Settings.Default.UpdateClientExecutablePath, String.Empty) }.Start();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Fatal exception occured", ex);

                Console.WriteLine("{0} [AppUpdate] -->> A fatal exception occured.", DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("{0} [AppUpdate] -->> Please submit the log file to the creator.", DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("{0} [AppUpdate] -->> Log file can be found in the 'Logs' directory.", DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("============= EXCEPTION ===============");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("============= EXCEPTION ===============");
                Console.WriteLine("{0} [AppUpdate] -->> Press ENTER to close the application.", DateTime.Now.ToString("HH:mm:ss"));
                Console.ReadLine();
            }
        }
    }
}
