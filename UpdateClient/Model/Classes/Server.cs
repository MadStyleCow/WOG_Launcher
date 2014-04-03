using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UpdateClient.Model.Enums;

namespace UpdateClient.Model.Classes
{
    public class Server
    {
        /* Fields */
        public Guid IdKey;
        public GameType Type;
        public String Name;
        public Int32 ThreadCount;
        public List<String> ManifestURLList;
        public List<String> ChangelogURLList;
        public List<String> ConfigExtensionList;
        public Boolean Beta;
        public String Hostname;
        public Int32 Port;
        public String Password;

        [XmlIgnore]
        public List<Addon> AddonList;
        [XmlIgnore]
        public List<Mod> ModList;
        [XmlIgnore]
        public List<String> FileList;
        [XmlIgnore]
        public String BaseManifestURL;

        /* Public methods */
        public async Task<string> GetManifestURL()
        {
            List<string> AvailableURL = new List<string>();
            Random RandomGenerator = new Random();

            foreach (string ManifestURL in ManifestURLList)
            {
                FtpWebRequest CheckManifestURL = (FtpWebRequest)WebRequest.Create(ManifestURL);
                CheckManifestURL.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse CheckManifestURLResponse = (FtpWebResponse)CheckManifestURL.GetResponse();

                if (CheckManifestURLResponse.StatusCode == FtpStatusCode.FileStatus)
                {
                    AvailableURL.Add(ManifestURL);
                }

                CheckManifestURLResponse.Close();
            }
            return AvailableURL[RandomGenerator.Next(0, AvailableURL.Count)];
        }

        public async Task GetAddonList(String pManifestURL)
        {
            String ServerManifestTemp = Path.GetTempFileName();
            String ServerAddonListTemp = Path.GetTempFileName();
            String ServerModListTemp = Path.GetTempFileName();

            try
            {
                // Download files
                Utilities.NetworkUtilities.DownloadToFile(pManifestURL, ServerManifestTemp);

                // Extract files
                Utilities.FileUtilities.ExtractArchive(ServerManifestTemp, @"addons/Addons.xml", ServerAddonListTemp);
                Utilities.FileUtilities.ExtractArchive(ServerManifestTemp, @"addons/Mods.xml", ServerModListTemp);

                // Serialize them
                DSServer DSAddonServer = new DSServer()
                {
                    Addons = ((DSServer)Utilities.XMLSerializer.XmlDeserializeFromFile(ServerAddonListTemp, typeof(DSServer))).Addons,
                    Mods = ((DSServer)Utilities.XMLSerializer.XmlDeserializeFromFile(ServerModListTemp, typeof(DSServer))).Mods
                };

                // Assign values
                AddonList = Utilities.Converters.ToAddonList(DSAddonServer.Addons, pManifestURL.Substring(0, pManifestURL.LastIndexOf('/')));
                ModList = Utilities.Converters.ToModList(DSAddonServer.Mods);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (File.Exists(ServerManifestTemp))
                    File.Delete(ServerManifestTemp);

                if (File.Exists(ServerAddonListTemp))
                    File.Delete(ServerAddonListTemp);

                if (File.Exists(ServerModListTemp))
                    File.Delete(ServerModListTemp);
            }
        }

        public string GetBaseDirectory()
        {
            switch (Type)
            {
                case GameType.ARMA2:
                    if (Properties.Settings.Default.A2_Path != String.Empty)
                        return Path.GetDirectoryName(Properties.Settings.Default.A2_Path);
                    else
                        return String.Empty;
                case GameType.ARMA3:
                    if (Properties.Settings.Default.A3_Path != String.Empty)
                        return Path.GetDirectoryName(Properties.Settings.Default.A3_Path);
                    else
                        return String.Empty;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetLaunchPath()
        {
            switch (Type)
            {
                case GameType.ARMA2:
                    return Properties.Settings.Default.A2_Path;
                case GameType.ARMA3:
                    return Properties.Settings.Default.A3_Path;
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<String> GetChangelogURL()
        {
            try
            {
                List<string> AvailableURL = new List<string>();
                Random RandomGenerator = new Random();

                foreach (string ChangelogURL in ChangelogURLList)
                {
                    HttpWebRequest CheckChangelogURL = (HttpWebRequest)WebRequest.Create(ChangelogURL);
                    HttpWebResponse CheckChangelogURLResponse = (HttpWebResponse)CheckChangelogURL.GetResponse();

                    if (CheckChangelogURLResponse.StatusCode == HttpStatusCode.OK)
                    {
                        AvailableURL.Add(ChangelogURL);
                    }

                    CheckChangelogURLResponse.Close();
                }
                return AvailableURL[RandomGenerator.Next(0, AvailableURL.Count)];
            }
            catch(Exception)
            {
                return "http://www.wogames.info";
            }
        }

        public bool UpToDate()
        {
            // TODO Check if any files are to be deleted.
            if (AddonList == null || FileList == null)
                return false;

            if (AddonList.Any(p => !p.Status) || FileList.Count != 0)
                return false;

            return true;
        }

        public List<String> GetFileList()
        {
            // Set base directory
            String wtf = GetBaseDirectory();

            Directory.SetCurrentDirectory(GetBaseDirectory());

            // Prepare
            List<String> FileSystemEntries = new List<string>();

            foreach (Mod ServerMod in ModList)
            {
                if (Directory.Exists(ServerMod.Name))
                {
                    foreach (String EntryPath in Directory.GetFileSystemEntries(ServerMod.Name, "*", SearchOption.AllDirectories))
                    {
                        FileAttributes Attributes = File.GetAttributes(EntryPath);

                        if ((Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            // If Directory
                            if(!AddonList.Any(p => p.RelativePath.Contains(EntryPath)))
                            {
                                FileSystemEntries.Add(EntryPath);
                            }
                        }
                        else
                        {
                            // If file
                            if (!AddonList.Any(p => p.RelativePath.StartsWith(Path.GetDirectoryName(EntryPath))) || !AddonList.Any(p => p.Name == Path.GetFileName(EntryPath)))
                            {
                                FileSystemEntries.Add(EntryPath);
                            }
                        }
                    }
                }
            }

            return FileSystemEntries;
        }
    }
}
