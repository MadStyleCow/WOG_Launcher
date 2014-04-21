using System;
using System.IO;
using System.Reflection;
using Client.Core.Utilities.Classes;
using Client.Properties;
using log4net;

namespace Client.Core.Utilities
{
    class FileCache
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileCache));

        /* Static fields */
        public static readonly FileCache Instance = new FileCache();

        /* Private fields */
        FileCacheEntryList CacheEntryList { get; set; }
        String FileCacheLocation { get; set; }

        /* Constructors */
        private FileCache()
        {
            // Set the base directory
// ReSharper disable once AssignNullToNotNullAttribute
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            if (File.Exists(Settings.Default.LocalFileCache))
            {
                FileCacheLocation = Settings.Default.LocalFileCache;
                CacheEntryList = (FileCacheEntryList)XMLSerializer.XmlDeserializeFromFile(Settings.Default.LocalFileCache, typeof(FileCacheEntryList));
            }
        }

        /* Methods */
        public bool Contains(String pPath)
        {
            try
            {
                var index = 0;
                while(index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[index].Path.Equals(pPath))
                    {
                        return true;
                    }
                    index++;
                }
                return false;
            }
            catch(Exception ex)
            {
                Logger.Error("An error occured while trying to find an entry in the file cache.", ex);
                throw;
            }
        }

        public FileCacheEntry Get(String pPath)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                var index = 0;
                while (index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[index].Path.Equals(pPath))
                    {
                        return CacheEntryList.FileCacheEntries[index];
                    }
                    index++;
                }
                throw new ApplicationException("No such path found");
            }
            catch(Exception ex)
            {
                Logger.Error("An error occured while trying to get an entry from the file cache.", ex);
                throw;
            }
        }

        public void Update(String pPath, String pHash, DateTime pLastWrite)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                var index = 0;
                while (index < CacheEntryList.FileCacheEntries.Count)
                {
                    if (CacheEntryList.FileCacheEntries[index].Path.Equals(pPath))
                    {
                        CacheEntryList.FileCacheEntries[index].Hash = pHash;
                        CacheEntryList.FileCacheEntries[index].LastWrite = pLastWrite;
                        break;
                    }
                    index++;
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error occured while trying to update a record in the file cache.", ex);
                throw;
            }
        }

        public void Add(String pPath, String pHash, DateTime pLastWrite)
        {
            try
            {
                CacheEntryList.FileCacheEntries.Add(new FileCacheEntry { Path = pPath, Hash = pHash, LastWrite = pLastWrite });
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to add a new record to the file cache.", ex);
                throw;
            }
            
        }

        public void Write()
        {
            try
            {
                // Set the directory
// ReSharper disable once AssignNullToNotNullAttribute
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                // Write the file.
                File.WriteAllText(FileCacheLocation, CacheEntryList.XmlSerializeToString());
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to write the file cache to a file.", ex);
                throw;
            }

        }
    }
}
