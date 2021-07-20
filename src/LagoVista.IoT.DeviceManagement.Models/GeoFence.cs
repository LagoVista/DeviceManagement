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
