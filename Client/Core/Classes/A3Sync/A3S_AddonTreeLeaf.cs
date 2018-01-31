using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class A3S_AddonTreeLeaf
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// SHA-1 hash sum for the remote file
        /// </summary>
        [JsonProperty("sha1")]
        public string Sha1Hash { get; set; }

        /// <summary>
        /// Size of the original remote file (in bytes)
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Size of the compressed remote file (in bytes). Used if archiving option is selected during repository building.
        /// </summary>
        [JsonProperty("compressedSize")]
        public int CompressedSize { get; set; }

        /// <summary>
        /// Unknown property.
        /// </summary>
        [JsonProperty("complete")]
        public int Complete { get; set; }

        /// <summary>
        /// Unknown property.
        /// </summary>
        [JsonProperty("destinationPath")]
        public string DestinationPath { get; set; }

        /// <summary>
        /// Unknown property.
        /// </summary>
        [JsonProperty("localSHA1")]
        public object LocalSHA1Hash { get; set; }

        /// <summary>
        /// A boolean, indicating whether the file has been updated.
        /// From last revision? During the last build?
        /// </summary>
        [JsonProperty("updated")]
        public bool Updated { get; set; }

        /// <summary>
        /// A boolean, indicating whether the file has been deleted.
        /// From last revision? During the last build?
        /// </summary>
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// A boolean, indicating whether the file was compressed when building.
        /// </summary>
        [JsonProperty("compressed")]
        public bool Compressed { get; set; }

        /// <summary>
        /// A string, containing the relative path, where the file should be located (in the local FS).
        /// </summary>
        [JsonProperty("relativePath")]
        public string RelativePath { get; set; }

        /// <summary>
        /// A boolean, indicating whether this is a leaf node (has no children).
        /// </summary>
        [JsonProperty("leaf")]
        public bool HasChildren { get; set; }

        /// <summary>
        /// A list of files within this tree node.
        /// </summary>
        [JsonProperty("list")]
        public List<A3S_AddonTreeLeaf> Children { get; set; }

        /// <summary>
        /// A boolean, indicating whether this file is an addon. Is used only for mod entries.
        /// </summary>
        [JsonProperty("markAsAddon")]
        public bool MarkAsAddon { get; set; }

        /// <summary>
        /// A boolean, indicating whether this node should be hidden.
        /// </summary>
        [JsonProperty("hidden")]
        public bool Hidden { get; set; }
    }

}
