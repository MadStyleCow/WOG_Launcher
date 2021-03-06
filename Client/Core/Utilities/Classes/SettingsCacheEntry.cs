﻿using System;
using System.Collections.Generic;

namespace Client.Core.Utilities.Classes
{
    [Serializable]
    public class SettingsCacheEntry
    {
        /* Fields */
        public Guid ServerIdKey { get; set; }
        public List<String> ModList { get; set; }
        public Boolean NoSplash { get; set; }
        public Boolean ShowScriptErrors { get; set; }
        public Boolean WinXp { get; set; }
        public Boolean Windowed { get; set; }
        public Boolean EmptyWorld { get; set; }
        public Boolean CpuCountChecked { get; set; }
        public Int32 CpuCount { get; set; }
        public Boolean MaxMemoryChecked { get; set; }
        public Int32 MaxMemory { get; set; }
        public Boolean ExThreadsChecked { get; set; }
        public Int32 ExThreads { get; set; }
        public String AdditionalParameters;
        public Boolean AutoConnect { get; set; }
    }

    [Serializable]
    public class SettingsCacheEntryList
    {
        /* Fields */
        public List<SettingsCacheEntry> SettingsCacheEntries { get; set; }

        /* Constructors */
    }
}
