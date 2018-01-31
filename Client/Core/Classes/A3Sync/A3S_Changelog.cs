using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class A3S_Changelog
    {
        // Public properties
        /// <summary>
        /// A list of changelog entries.
        /// </summary>
        [JsonProperty("list")]
        public List<A3S_ChangelogEntry> Entries { get; set; }

        // Public constructor
        public A3S_Changelog() { }
    }
}
