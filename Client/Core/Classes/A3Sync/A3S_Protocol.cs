using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class A3S_Protocol
    {
        // Properties
        /// <summary>
        /// Login name to be used when connecting to the repository.
        /// </summary>
        [JsonProperty("login")]
        public string Username { get; set; }

        /// <summary>
        /// Password to be used when connecting to the repository.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// URL of the repository. Protocol not specified.
        /// </summary>
        [JsonProperty("url")]
        public string RepositoryURL { get; set; }

        /// <summary>
        /// Port to be used when connecting to the repository.
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// URI Scheme to be used
        /// </summary>
        [JsonProperty("protocolType")]
        public string Scheme { get; set; }

        /// <summary>
        /// Connection timeout.
        /// </summary>
        [JsonProperty("connectionTimeOut")]
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Read timeout.
        /// </summary>
        [JsonProperty("readTimeOut")]
        public int readTimeout { get; set; }

        /// <summary>
        /// Hostname of the repository's server.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// Remote, relative path to the folder with the repository.
        /// </summary>
        [JsonProperty("remotePath")]
        public string RelativePath { get; set; }

        // Public constructor
        public A3S_Protocol() { }
    }
}
