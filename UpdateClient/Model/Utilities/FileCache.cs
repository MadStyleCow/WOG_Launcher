using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UpdateClient.Model.Utilities.Classes;
using System.Windows;

namespace UpdateClient.Model.Utilities
{
    class FileCache
    {
        /* Static fields */
        public static FileCache Instance = new FileCache();

        /* Private fields */
        FileCacheEntryList CacheEntryList;
        FileCacheEntryList NewCacheEntryList;
        String FileCacheLocation;

        /* Constructors */
        public FileCache()
        {
            // Set the base directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            if (File.Exists(Properties.Settings.Default.FileCache))
            {
                this.FileCacheLocation = Properties.Settings.Default.FileCache;
                this.CacheEntryList = (FileCacheEntryList)XMLSerializer.XmlDeserializeFromFile(Properties.Settings.Default.FileCache, typeof(FileCacheEntryList));
                this.NewCacheEntryList = new FileCacheEntryList();
                this.NewCacheEntryList.FileCacheEntries = new List<FileCacheEntry>(CacheEntryList.FileCacheEntries);
            }
        }

        /* Methods */
        public bool Contains(String pPath)
        {
            return CacheEntryList.FileCacheEntries.Any(p => p.Path == pPath);
        }

        public FileCacheEntry Get(String pPath)
        {
            return CacheEntryList.FileCacheEntries.Find(p => p.Path == pPath);
        }

        public void Update(String pPath, String pHash, DateTime pLastWrite)
        {
            FileCacheEntry Entry = CacheEntryList.FileCacheEntries.Find(p => p.Path == pPath);
            Entry.Hash = pHash;
            Entry.LastWrite = pLastWrite;
        }

        public void Add(String pPath, String pHash, DateTime pLastWrite)
        {
            NewCacheEntryList.FileCacheEntries.Add(new FileCacheEntry() { Path = pPath, Hash = pHash, LastWrite = pLastWrite });
        }

        public void Write()
        {
            // Set the directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            // Write the file.
            File.WriteAllText(FileCacheLocation, NewCacheEntryList.XmlSerializeToString());
        }
    }
}
