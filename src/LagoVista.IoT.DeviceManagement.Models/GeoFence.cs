using LagoVista.Core.Models.Geo;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class GeoFence
    {
        public GeoLocation Center { get; set; }

        public double RadiusMeters { get; set; }
    }
}
