using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class ServerInfo
    {
        public int revision { get; set; }
        public long buildDate { get; set; }
        public int numberOfFiles { get; set; }
        public long totalFilesSize { get; set; }
        public List<object> hiddenFolderPaths { get; set; }
        public int numberOfConnections { get; set; }
        public bool noPartialFileTransfer { get; set; }
        public bool repositoryContentUpdated { get; set; }
        public bool compressedPboFilesOnly { get; set; }
    }
}
