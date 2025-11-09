// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7fb73a81a81d4e16657da61bb9a09f9d1fd93632969212022a8d385ddf37abdc
// IndexVersion: 2
// --- END CODE INDEX META ---
using Newtonsoft.Json;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DevicePEMRequest
    {
        [JsonProperty("pem_uri")]
        public string PEM_URI { get; set; }
    }
}
