using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UpdateClient.Core.Enums;

namespace UpdateClient.Core.Classes
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
        public async Task GetAddonList(String pManifestURL)
        {
            String ServerManifestTemp = Path.GetTempFileName();
            String ServerAddonListTemp = Path.GetTempFileName();
            String ServerModListTemp = Path.GetTempFileName();

            try
            {
                // Download files
                await Utilities.NetworkUtilities.DownloadToFile(pManifestURL, ServerManifestTemp);

                // Extract files
                await Utilities.FileUtilities.ExtractArchive(ServerManifestTemp, @"addons/Addons.xml", ServerAddonListTemp);
                await Utilities.FileUtilities.ExtractArchive(ServerManifestTemp, @"addons/Mods.xml", ServerModListTemp);

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
                            if(!AddonList.Any(p => p.RelativePath.ToLowerInvariant().Contains(EntryPath.ToLowerInvariant())))
                            {
                                FileSystemEntries.Add(EntryPath);
                            }
                        }
                        else
                        {
                            // If file
                            if (!AddonList.Any(p => p.RelativePath.StartsWith(Path.GetDirectoryName(EntryPath), StringComparison.InvariantCultureIgnoreCase)) || !AddonList.Any(p => p.Name.ToLowerInvariant() == Path.GetFileName(EntryPath).ToLowerInvariant()))
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
