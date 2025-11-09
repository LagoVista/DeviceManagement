// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9f3093119cd3625d11421037affbedae58293e23de0e0266b4a7076a041d2a26
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.Geo;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceMedia
    {
        public string ItemId { get; set; }
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public GeoLocation Location { get; set; }
    }
}
