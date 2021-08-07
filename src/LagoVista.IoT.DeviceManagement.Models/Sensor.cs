using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    /// <summary>
    /// THis is a specific sensor on a board.
    /// </summary>
    public class Sensor
    {
        public Sensor()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        /// <summary>
        /// BlueTooth Address
        /// </summary>
        public string Address { get; set; }

        public EntityHeader<ParameterTypes> AttributeType { get; set; }

        public SensorTechnology Technology { get; set; }

        public string LastUpdated { get; set; }

        public EntityHeader<SensorDefinition> SensorDefinition { get; set; }

        /// <summary>
        /// Type of Sensor 
        /// </summary>
        public byte SensorTypeId { get; set; } = 0;

        public int PortIndex { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double DeviceScaler { get; set; } = 1;
        public double Calibration { get; set; } = 1;
        public double Zero { get; set; } = 0;

        public double LowThreshold { get; set; }
        public double HighTheshold { get; set; }

        public string LowValueErrorCode { get; set; }
        public string HighValueErrorCode { get; set; }

        public bool AlertsEnabled { get; set; }

        public SensorStates State { get; set; }
        public EntityHeader<UnitSet> UnitSet { get; set; }

        public string Value { get; set; }
    }
}
