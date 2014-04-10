using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UpdateClient.Core.Utilities.Classes;
using System.Windows;

namespace UpdateClient.Core.Utilities
{
    class FileCache
    {
        /* Static fields */
        public static FileCache Instance = new FileCache();

        /* Private fields */
        FileCacheEntryList CacheEntryList { get; set; }
        String FileCacheLocation { get; set; }

        /* Constructors */
        public FileCache()
        {
            // Set the base directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            if (File.Exists(Properties.Settings.Default.LocalFileCache))
            {
                this.FileCacheLocation = Properties.Settings.Default.LocalFileCache;
                this.CacheEntryList = (FileCacheEntryList)XMLSerializer.XmlDeserializeFromFile(Properties.Settings.Default.LocalFileCache, typeof(FileCacheEntryList));
            }
        }

        /* Methods */
        public bool Contains(String pPath)
        {
            try
            {
                int Index = 0;
                while(Index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[Index].Path.Equals(pPath))
                    {
                        return true;
                    }
                    Index++;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public FileCacheEntry Get(String pPath)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                int Index = 0;
                while (Index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[Index].Path.Equals(pPath))
                    {
                        return CacheEntryList.FileCacheEntries[Index];
                    }
                    Index++;
                }
                throw new ApplicationException("No such path found");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Update(String pPath, String pHash, DateTime pLastWrite)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                int Index = 0;
                while (Index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[Index].Path.Equals(pPath))
                    {
                        CacheEntryList.FileCacheEntries[Index].Hash = pHash;
                        CacheEntryList.FileCacheEntries[Index].LastWrite = pLastWrite;
                        break;
                    }
                    Index++;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Add(String pPath, String pHash, DateTime pLastWrite)
        {
            CacheEntryList.FileCacheEntries.Add(new FileCacheEntry() { Path = pPath, Hash = pHash, LastWrite = pLastWrite });
        }

        public void Write()
        {
            // Set the directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            // Write the file.
            File.WriteAllText(FileCacheLocation, CacheEntryList.XmlSerializeToString());
        }
    }
}
