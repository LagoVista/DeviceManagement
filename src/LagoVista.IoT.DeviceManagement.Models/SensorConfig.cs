using LagoVista.Core;
using LagoVista.Core.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{    

    public class SensorConfig   : ModelBase
    {
        public SensorConfig()
        {
            Id = Guid.NewGuid().ToId();
            Class = SensorValueType.Number;
        }

        public string Id { get; set; }

        /// <summary>
        /// BlueTooth Address
        /// </summary>
        public string Address { get; set; }
        
        public SensorValueType Class { get; set; }

        /// <summary>
        /// Type of Sensor
        /// </summary>
        public byte Config { get; set; } = 0;
        
        public int SensorIndex { get; set; }

        public string Key { get; set; }


        public string Name { get; set; }
        public string Description { get; set; }
        public string Units { get; set; }
        
        public double DeviceScaler { get; set; } = 1;
        public double Calibration { get; set; } = 1;
        public double Zero { get; set; } = 0;

        public double LowThreshold { get; set; }
        public double HighTheshold { get; set; }

        public string LowValueErrorCode { get; set; }
        public string HighValueErrorCode { get; set; }

        public bool AlertsEnabled { get; set; }

        public SensorStates State { get; set; }
    }
}
