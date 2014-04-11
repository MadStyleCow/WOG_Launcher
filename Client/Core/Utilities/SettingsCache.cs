using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Core.Utilities.Classes;

namespace Client.Core.Utilities
{
    class SettingsCache
    {
         /* Static fields */
        public static SettingsCache Instance = new SettingsCache(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.LocalSettingsCache);

        /* Private fields */
        SettingsCacheEntryList CacheEntryList;
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
            }
        }

        /* Methods */
        public bool Contains(Guid pIdKey)
        {
            try
            {
                int Index = 0;
                while (Index < CacheEntryList.SettingsCacheEntries.Count)
                {
                    if (CacheEntryList.SettingsCacheEntries[Index].ServerIdKey.Equals(pIdKey))
                    {
                        return true;
                    }
                    Index++;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SettingsCacheEntry Get(Guid pIdKey)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                int Index = 0;
                while (Index < CacheEntryList.SettingsCacheEntries.Count)
                {
                    if (CacheEntryList.SettingsCacheEntries[Index].ServerIdKey.Equals(pIdKey))
                    {
                        return CacheEntryList.SettingsCacheEntries[Index];
                    }
                    Index++;
                }
                throw new ApplicationException("No such id key found");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(Guid pIdKey, SettingsCacheEntry pEntry)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                int Index = 0;
                while (Index < CacheEntryList.SettingsCacheEntries.Count)
                {
                    if (CacheEntryList.SettingsCacheEntries[Index].ServerIdKey.Equals(pIdKey))
                    {
                        CacheEntryList.SettingsCacheEntries[Index] = pEntry;
                        break;
                    }
                    Index++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void Add(SettingsCacheEntry pEntry)
        {
            CacheEntryList.SettingsCacheEntries.Add(pEntry);
        }

        public void Write()
        {
            // Set the directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            // Write the file
            File.WriteAllText(SettingsCacheLocation, CacheEntryList.XmlSerializeToString());
        }
    }
}
