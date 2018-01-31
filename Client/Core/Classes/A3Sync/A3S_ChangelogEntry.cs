using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class A3S_ChangelogEntry
    {
        /// <summary>
        /// ID of the revision.
        /// </summary>
        [JsonProperty("revision")]
        public int Revision { get; set; }

        /// <summary>
        /// Build date of the revision (unix time).
        /// </summary>
        [JsonProperty("buildDate")]
        public long BuildDate { get; set; }

        /// <summary>
        /// A boolean, indicating whether any content was updated during this build.
        /// </summary>
        [JsonProperty("contentUpdated")]
        public bool ContentUpdated { get; set; }

        /// <summary>
        /// A list of mods added during this build.
        /// </summary>
        [JsonProperty("newAddons")]
        public List<string> NewMods { get; set; }

        /// <summary>
        /// A list of mods updated during this build.
        /// </summary>
        [JsonProperty("updatedAddons")]
        public List<object> UpdatedMods { get; set; }

        /// <summary>
        /// A list of mods removed during this build.
        /// </summary>
        [JsonProperty("deletedAddons")]
        public List<object> DeletedMods { get; set; }

        /// <summary>
        /// List of mods present in the repository.
        /// </summary>
        [JsonProperty("addons")]
        public List<string> Mods { get; set; }
    }
}
