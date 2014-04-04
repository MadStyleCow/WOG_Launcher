using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateClient.Model.Utilities.Classes;

namespace UpdateClient.Model.Utilities
{
    class SettingsCache
    {
         /* Static fields */
        public static SettingsCache Instance = new SettingsCache(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.LocalSettingsCache);

        /* Private fields */
        SettingsCacheEntryList CacheEntryList;
        SettingsCacheEntryList NewCacheEntryList;
        String SettingsCacheLocation;

        /* Constructors */
        public SettingsCache(String pBaseDirectory, String pSettingsCacheLocation)
        {
            // Set the base directory
            Directory.SetCurrentDirectory(pBaseDirectory);

            if (File.Exists(pSettingsCacheLocation))
            {
                this.SettingsCacheLocation = pSettingsCacheLocation;
                this.CacheEntryList = (SettingsCacheEntryList)XMLSerializer.XmlDeserializeFromFile(SettingsCacheLocation, typeof(SettingsCacheEntryList));
                this.NewCacheEntryList = new SettingsCacheEntryList();
                this.NewCacheEntryList.SettingsCacheEntries = new List<SettingsCacheEntry>(CacheEntryList.SettingsCacheEntries);
            }
        }

        /* Methods */
        public bool Contains(Guid pIdKey)
        {
            return CacheEntryList.SettingsCacheEntries.Any(p => p.ServerIdKey == pIdKey);
        }

        public SettingsCacheEntry Get(Guid pIdKey)
        {
            return CacheEntryList.SettingsCacheEntries.Find(p => p.ServerIdKey == pIdKey);
        }

        public void Update(Guid pIdKey, SettingsCacheEntry pEntry)
        {
            CacheEntryList.SettingsCacheEntries[CacheEntryList.SettingsCacheEntries.FindIndex(p => p.ServerIdKey == pIdKey)] = pEntry;
        }
        
        public void Add(SettingsCacheEntry pEntry)
        {
            NewCacheEntryList.SettingsCacheEntries.Add(pEntry);
        }

        public void Write(String pPath)
        {
            File.WriteAllText(pPath, NewCacheEntryList.XmlSerializeToString());
        }
    }
}
