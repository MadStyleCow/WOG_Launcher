using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Client.Core.Enums;
using Client.Core.Utilities;

namespace Client.Core.Classes
{
    public class Server
    {
        /* Fields */
        public Guid IdKey;
        public GameType Type;
        public RepositoryType RepositoryType;
        public String Name;
        public Int32 ThreadCount;
        public List<String> ManifestUrlList;
        public List<String> ChangelogUrlList;
        public List<String> ConfigExtensionList;
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
        public String BaseManifestUrl;

        /* Public methods */
        public async Task GetData(String pManifestUrl)
        {
            var serverManifestTemp = Path.GetTempFileName();
            var serverAddonListTemp = Path.GetTempFileName();
            var serverModListTemp = Path.GetTempFileName();

            try
            {
                // Download files
                NetworkUtilities.DownloadToFile(pManifestUrl, serverManifestTemp);

                // Extract files
                FileUtilities.ExtractArchive(serverManifestTemp, @"addons/Addons.xml", serverAddonListTemp);
                FileUtilities.ExtractArchive(serverManifestTemp, @"addons/Mods.xml", serverModListTemp);

                // Serialize them
                var dsAddonServer = new DSServer
                {
                    Addons = ((DSServer)XMLSerializer.XmlDeserializeFromFile(serverAddonListTemp, typeof(DSServer))).Addons,
                    Mods = ((DSServer)XMLSerializer.XmlDeserializeFromFile(serverModListTemp, typeof(DSServer))).Mods
                };

                // Assign values
                AddonList = Converters.ToAddonList(dsAddonServer.Addons);
                ModList = Converters.ToModList(dsAddonServer.Mods);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (File.Exists(serverManifestTemp))
                {
                    File.Delete(serverManifestTemp);
                }

                if (File.Exists(serverAddonListTemp))
                {
                    File.Delete(serverAddonListTemp);
                }

                if (File.Exists(serverModListTemp))
                { 
                    File.Delete(serverModListTemp);
                }
            }
        }

        public async Task<Boolean> AddonSetValid()
        {
            if ((AddonList != null) && (FileList != null))
            {
                if (AddonList.Any(p => !p.Status) || !FileList.Count.Equals(0))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public async Task<List<String>> GetFileDeleteList()
        {
            // Set base directory
            Directory.SetCurrentDirectory(LocalMachine.Instance.GetModDirectory(Type));

            // Prepare
            var fileSystemEntries = new List<string>();

            foreach (var serverMod in ModList)
            {
                if (Directory.Exists(serverMod.Name))
                {
                    foreach (var entryPath in Directory.GetFileSystemEntries(serverMod.Name, "*", SearchOption.AllDirectories))
                    {
                        var attributes = File.GetAttributes(entryPath);

                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            // If Directory
                            if(!AddonList.Any(p => p.RelativePath.ToLowerInvariant().Contains(entryPath.ToLowerInvariant())))
                            {
                                fileSystemEntries.Add(entryPath);
                            }
                        }
                        else
                        {
                            // If file
                            if (!AddonList.Any(p => p.RelativePath.StartsWith(Path.GetDirectoryName(entryPath), StringComparison.InvariantCultureIgnoreCase)) || !AddonList.Any(p =>
                            {
                                var fileName = Path.GetFileName(entryPath);
                                return fileName != null && p.Name.ToLowerInvariant() == fileName.ToLowerInvariant();
                            }))
                            {
                                fileSystemEntries.Add(entryPath);
                            }
                        }
                    }
                }
            }

            return fileSystemEntries;
        }
    }
}
