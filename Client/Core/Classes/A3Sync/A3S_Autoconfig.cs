using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class A3S_Autoconfig
    {
        // Public properties
        /// <summary>
        /// Name of the repository.
        /// </summary>
        [JsonProperty("repositoryName")]
        public string Name { get; set; }

        /// <summary>
        /// An object containing the connection information for the repository.
        /// </summary>
        [JsonProperty("protocole")]
        public A3S_Protocol Details { get; set; }

        /// <summary>
        /// A list of favorite servers.
        /// </summary>
        [JsonProperty("favoriteServers")]
        public List<object> Favorites { get; set; }

        // Public constructor
        public A3S_Autoconfig() { }
    }
}