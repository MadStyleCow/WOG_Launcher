using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using log4net;

namespace Client.Core.Utilities
{
    public static class FileUtilities
    {
        // Logger
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Calculates the MD5 hash for the indicated file.
        /// </summary>
        /// <param name="pFilePath">File to be checked</param>
        /// <returns>Hash sum of the file</returns>
        public static async Task<String> CalculateHash(String pFilePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(pFilePath))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToUpper();
                    }
                }
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch(Exception ex)
            {
                Logger.Error(String.Format("Exception caught while trying to calculate hashsum for file '{0}'", pFilePath), ex);
                throw;
            }
        }

        /// <summary>
        /// Extracts the indicated entry from a ZIP archive and places it into an indicated place.
        /// </summary>
        /// <param name="pFilePath">Path to the ZIP archive</param>
        /// <param name="pEntry">File to be extracted</param>
        /// <param name="pOutput">Path to the result file</param>
        /// <returns></returns>
        public static async Task ExtractArchive(String pFilePath, String pEntry, String pOutput)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(pOutput)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pOutput));
                }

                using (var stream = File.OpenRead(pFilePath))
                {
                    new ZipArchive(stream).GetEntry(pEntry).ExtractToFile(pOutput, true);
                }
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch(Exception ex)
            {
                Logger.Error(String.Format("Exception caught while trying to extract file '{0}'", pFilePath), ex);
                throw;
            }
        }

        /// <summary>
        /// Attempts to delete an indicated record from the file system.
        /// </summary>
        /// <param name="pFilePath">Record to be deleted.</param>
        /// <returns></returns>
        public static async Task DeleteFile(String pFilePath)
        {
            try
            {
                var attr = File.GetAttributes(pFilePath);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var info = new DirectoryInfo(pFilePath);
                    info.Attributes &= ~FileAttributes.ReadOnly;
                    info.Refresh();
                    info.Delete(true);
                }
                else
                {
                    var info = new FileInfo(pFilePath);
                    info.Attributes &= ~FileAttributes.ReadOnly;
                    info.Refresh();
                    info.Delete();
                }
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("Exception caught while trying to delete file '{0}'", pFilePath), ex);
                throw;
            }
        }
    }
}
