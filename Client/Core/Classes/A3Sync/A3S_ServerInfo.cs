using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    /// <summary>
    /// This class describes the current state of the repository.
    /// </summary>
    [Serializable]
    public class A3S_ServerInfo
    {
        /// <summary>
        /// Current revision of the repository.
        /// </summary>
        [JsonProperty("revision")]
        public int Revision { get; set; }

        /// <summary>
        /// Last build date of the repository (unix time).
        /// </summary>
        [JsonProperty("buildDate")]
        public long BuildDate { get; set; }

        /// <summary>
        /// Amount of files within the repository.
        /// </summary>
        [JsonProperty("numberOfFiles")]
        public int FileCount { get; set; }

        /// <summary>
        /// Total size of the repository (in bytes)
        /// </summary>
        [JsonProperty("totalFilesSize")]
        public long TotalSize { get; set; }

        /// <summary>
        /// Unknown property.
        /// </summary>
        [JsonProperty("hiddenFolderPaths")]
        public List<string> hiddenFolderPaths { get; set; }

        /// <summary>
        /// Maximum number of connections this repository allows.
        /// </summary>
        [JsonProperty("numberOfConnections")]
        public int ConnectionCount { get; set; }

        /// <summary>
        /// A boolean, indicating whether partial file transfer is supported.
        /// </summary>
        [JsonProperty("noPartialFileTransfer")]
        public bool PartialTransferAvailable { get; set; }

        /// <summary>
        /// A boolean, indicating whether the repository content was updated (from the last build).
        /// </summary>
        [JsonProperty("repositoryContentUpdated")]
        public bool ContentChanged { get; set; }

        /// <summary>
        /// A boolean, indicating whether the repository consists of compressed files.
        /// </summary>
        [JsonProperty("compressedPboFilesOnly")]
        public bool Compressed { get; set; }
    }
}
