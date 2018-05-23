using Newtonsoft.Json;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DevicePEMRequest
    {
        [JsonProperty("pem_uri")]
        public string PEM_URI { get; set; }
    }
}
