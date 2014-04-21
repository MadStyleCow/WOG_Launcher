using System;
using System.IO;
using System.Reflection;
using Client.Core.Utilities.Classes;
using Client.Properties;
using log4net;

namespace Client.Core.Utilities
{
    class SettingsCache
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsCache));

         /* Static fields */
        public static readonly SettingsCache Instance = new SettingsCache(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.LocalSettingsCache);

        /* Private fields */
        readonly SettingsCacheEntryList _cacheEntryList;
        readonly String _settingsCacheLocation;

        /* Constructors */

        private SettingsCache(String pBaseDirectory, String pSettingsCacheLocation)
        {
            // Set the base directory
            Directory.SetCurrentDirectory(pBaseDirectory);

            if (File.Exists(pSettingsCacheLocation))
            {
                _settingsCacheLocation = pSettingsCacheLocation;
                _cacheEntryList = (SettingsCacheEntryList)XMLSerializer.XmlDeserializeFromFile(_settingsCacheLocation, typeof(SettingsCacheEntryList));
            }
        }

        /* Methods */
        public bool Contains(Guid pIdKey)
        {
            try
            {
                var index = 0;
                while (index < _cacheEntryList.SettingsCacheEntries.Count)
                {
                    if (_cacheEntryList.SettingsCacheEntries[index].ServerIdKey.Equals(pIdKey))
                    {
                        return true;
                    }
                    index++;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to find an entry in the settings cache.", ex);
                throw;
            }
        }

        public SettingsCacheEntry Get(Guid pIdKey)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                var index = 0;
                while (index < _cacheEntryList.SettingsCacheEntries.Count)
                {
                    if (_cacheEntryList.SettingsCacheEntries[index].ServerIdKey.Equals(pIdKey))
                    {
                        return _cacheEntryList.SettingsCacheEntries[index];
                    }
                    index++;
                }
                throw new ApplicationException("No such id key found");
            }
            catch (Exception ex)
            {
                Logger.Error("An error was encountered while trying to get an entry from the settings cache", ex);
                throw;
            }
        }

        public void Update(Guid pIdKey, SettingsCacheEntry pEntry)
        {
            try
            {
                // Use while instead of foreach / extensions as those are mutable
                var index = 0;
                while (index < _cacheEntryList.SettingsCacheEntries.Count)
                {
                    if (_cacheEntryList.SettingsCacheEntries[index].ServerIdKey.Equals(pIdKey))
                    {
                        _cacheEntryList.SettingsCacheEntries[index] = pEntry;
                        break;
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to update a record in the settings cache.", ex);
                throw;
            }
        }
        
        public void Add(SettingsCacheEntry pEntry)
        {
            try
            {
                _cacheEntryList.SettingsCacheEntries.Add(pEntry);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to add a new record to the settings cache.", ex);
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

                // Write the file
                File.WriteAllText(_settingsCacheLocation, _cacheEntryList.XmlSerializeToString());
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while trying to write the file cache to a file.", ex);
                throw;
            }
        }
    }
}
