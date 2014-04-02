using System;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;

namespace UpdateClient.Model.Utilities
{
    public static class FileUtilities
    {
        public static String CalculateHash(String pFilePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(pFilePath))
                {
                   return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToUpper();
                }
            }
        }

        public static bool ExtractArchive(String pFilePath, String pEntry, String pOutput)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(pOutput)))
                    Directory.CreateDirectory(Path.GetDirectoryName(pOutput));

                using (FileStream Stream = File.OpenRead(pFilePath))
                {
                    new ZipArchive(Stream).GetEntry(pEntry).ExtractToFile(pOutput, true);
                }
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public static bool DeleteFile(String pFilePath)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(pFilePath);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (String file in Directory.GetFiles(pFilePath, "*", SearchOption.AllDirectories))
                    {
                        FileInfo info = new FileInfo(file);
                        info.IsReadOnly = false;
                    }
                    Directory.Delete(pFilePath, true);
                }
                else
                {
                    FileInfo info = new FileInfo(pFilePath);
                    info.IsReadOnly = false;
                    File.Delete(pFilePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }
    }
}
