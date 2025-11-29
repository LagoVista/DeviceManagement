using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class SensorDataArchive : TableStorageEntity
    {
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }

        public double? ADCSensor1 { get; set; }

        public double? ADCSensor2 { get; set; }
        public double? ADCSensor3 { get; set; }
        public double? ADCSensor4 { get; set; }
        public double? ADCSensor5 { get; set; }
        public double? ADCSensor6 { get; set; }
        public double? ADCSensor7 { get; set; }
        public double? ADCSensor8 { get; set; }

        public double? IoSensor1 { get; set; }
        public double? IoSensor2 { get; set; }
        public double? IoSensor3 { get; set; }
        public double? IoSensor4 { get; set; }
        public double? IoSensor5 { get; set; }
        public double? IoSensor6 { get; set; }
        public double? IoSensor7 { get; set; }
        public double? IoSensor8 { get; set; }
    }
}
