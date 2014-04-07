using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateClient.Model.Utilities;

namespace UpdateClient.Model.Classes
{
    public class Addon
    {
        public String Name;              // Name of the file
        public String RelativePath;      // Path from the base directory to the file's location
        public Int64 Size;               // Size of the file in bytes
        public String AbsoluteURL;       // URL of the file archive.
        public String Hash;              // MD5 hash of the file's contents
        public Boolean Status;           // Indicates whether the file is correct and has already been updated or does not require an update.

        /* Public methods */
        public async Task CheckAddon(String pBaseDirectory, List<String> pExtensions)
        {
            try
            {
                Directory.SetCurrentDirectory(pBaseDirectory);

                String RelativeAddonPath = String.Format("{0}\\{1}", RelativePath, Name);

                if (File.Exists(RelativeAddonPath))
                {
                    FileInfo AddonFileInfo = new FileInfo(RelativeAddonPath);
                    if (FileCache.Instance.Contains(RelativeAddonPath))
                    {
                        if (AddonFileInfo.LastWriteTimeUtc == FileCache.Instance.Get(RelativeAddonPath).LastWrite)
                        {
                            if (!pExtensions.Contains(AddonFileInfo.Extension.ToUpperInvariant()))
                                Status = (FileCache.Instance.Get(RelativeAddonPath).Hash == Hash);
                            else
                                Status = true;
                        }
                        else
                        {
                            String NewHash = FileUtilities.CalculateHash(RelativeAddonPath).Result;
                            FileCache.Instance.Update(RelativeAddonPath, NewHash, AddonFileInfo.LastWriteTimeUtc);
                            if (!pExtensions.Contains(AddonFileInfo.Extension.ToUpperInvariant()))
                                Status = (NewHash == Hash);
                            else
                                Status = true;
                        }
                    }
                    else
                    {
                        String NewHash = FileUtilities.CalculateHash(RelativeAddonPath).Result;
                        FileCache.Instance.Add(RelativeAddonPath, NewHash, AddonFileInfo.LastWriteTimeUtc);
                        if (!pExtensions.Contains(AddonFileInfo.Extension.ToUpperInvariant()))
                            Status = (NewHash == Hash);
                        else Status = true;
                    }
                }
                else
                {
                    Status = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAddon(String pBaseDirectory)
        {
            Directory.SetCurrentDirectory(pBaseDirectory);

            String TemporaryFile = Path.GetTempFileName();

            try
            {
                await NetworkUtilities.DownloadToFile(AbsoluteURL, TemporaryFile);
                await FileUtilities.ExtractArchive(TemporaryFile, Name, String.Format("{0}\\{1}", RelativePath, Name));
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (File.Exists(TemporaryFile))
                    File.Delete(TemporaryFile);
            }
        }
    }
}
