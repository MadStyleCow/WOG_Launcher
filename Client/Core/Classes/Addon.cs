using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Client.Core.Utilities;
using log4net;

namespace Client.Core.Classes
{
    public class Addon
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Addon));

        public String Name;              // Name of the file
        public String RelativePath;      // Path from the base directory to the file's location
        public Int64 Size;               // Size of the file in bytes
        public String RelativeUrl;       // URL of the file archive.
        public String Hash;              // MD5 hash of the file's contents
        public Boolean Status;           // Indicates whether the file is correct and has already been updated or does not require an update.

        /* Public methods */
        public async Task CheckAddon(String pBaseDirectory, List<String> pExtensions)
        {
            try
            {
                Directory.SetCurrentDirectory(pBaseDirectory);

                var relativeAddonPath = String.Format("{0}\\{1}", RelativePath, Name);

                if (File.Exists(relativeAddonPath))
                {
                    var addonFileInfo = new FileInfo(relativeAddonPath);
                    if (FileCache.Instance.Contains(relativeAddonPath))
                    {
                        if (addonFileInfo.LastWriteTimeUtc == FileCache.Instance.Get(relativeAddonPath).LastWrite)
                        {
                            if (!pExtensions.Contains(addonFileInfo.Extension.ToUpperInvariant()))
                                Status = (FileCache.Instance.Get(relativeAddonPath).Hash == Hash);
                            else
                                Status = true;
                        }
                        else
                        {
                            var newHash = await FileUtilities.CalculateHash(relativeAddonPath);
                            FileCache.Instance.Update(relativeAddonPath, newHash, addonFileInfo.LastWriteTimeUtc);
                            if (!pExtensions.Contains(addonFileInfo.Extension.ToUpperInvariant()))
                                Status = (newHash == Hash);
                            else
                                Status = true;
                        }
                    }
                    else
                    {
                        var newHash = await FileUtilities.CalculateHash(relativeAddonPath);
                        FileCache.Instance.Add(relativeAddonPath, newHash, addonFileInfo.LastWriteTimeUtc);
                        if (!pExtensions.Contains(addonFileInfo.Extension.ToUpperInvariant()))
                            Status = (newHash == Hash);
                        else Status = true;
                    }
                }
                else
                {
                    Status = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to check an addon.", ex);
                throw;
            }
        }

        public async Task<Boolean> UpdateAddon(String pBaseDirectory, String pBaseUrl)
        {
            var temporaryFile = Path.GetTempFileName();

            Directory.SetCurrentDirectory(pBaseDirectory);

            try
            {
                if (NetworkUtilities.DownloadToFile(String.Format("{0}/{1}", pBaseUrl, RelativeUrl), temporaryFile))
                {
                    await FileUtilities.ExtractArchive(temporaryFile, Name, String.Format("{0}\\{1}", RelativePath, Name));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error was encountered while trying to update an addon.", ex);
                throw;
            }
            finally
            {
                if (File.Exists(temporaryFile))
                {
                    File.Delete(temporaryFile);
                }
            }
        }
    }
}
