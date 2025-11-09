// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0cbcf3e27456ea82ecac8ede07cd10042e79b8e9519e559aa105a20b889df17d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models.Geo;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class GeoFence
    {
        public string Id { get; set; }

        public GeoFence()
        {
            Enabled = true;
            Id = Guid.NewGuid().ToId();
        }

        public string Name { get; set; }
        
        public bool IgnoreIfHasSecondaryLocation { get; set; }

        public bool Enabled { get; set; }

        public GeoLocation Center { get; set; }

        public double RadiusMeters { get; set; }
        
        public string Description { get; set; }
    }
}
