// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fc8ccd06172935e4eb048c60d8bcda7a3f197bd731ce59f43aafff849e966742
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public enum SensorStates
    {
        [EnumLabel(Sensor.Sensor_Offline, DeviceManagementResources.Names.SensorState_Offline, typeof(DeviceManagementResources))]
        Offline,

        [EnumLabel(Sensor.Sensor_Nominal, DeviceManagementResources.Names.SensorState_Nominal, typeof(DeviceManagementResources))]
        Nominal,

        [EnumLabel(Sensor.Sensor_Warning, DeviceManagementResources.Names.SensorState_Warning, typeof(DeviceManagementResources))]
        Warning,

        [EnumLabel(Sensor.Sensor_Error, DeviceManagementResources.Names.SensorState_Error, typeof(DeviceManagementResources))]
        Error,

        [EnumLabel(Sensor.Sensor_Off, DeviceManagementResources.Names.SensorState_Off, typeof(DeviceManagementResources))]
        Off,

        [EnumLabel(Sensor.Sensor_On, DeviceManagementResources.Names.SensorState_On, typeof(DeviceManagementResources))]
        On,

        [EnumLabel(Sensor.Sensor_AboceThreshold, DeviceManagementResources.Names.Sensor_AboceThreshold, typeof(DeviceManagementResources))]
        Error_Above,

        [EnumLabel(Sensor.Sensor_BeloweThreshold, DeviceManagementResources.Names.Sensor_BeloweThreshold, typeof(DeviceManagementResources))]
        Error_Below,
    }

    public class SensorSummary : ModelBase
    {
        EntityHeader<SensorStates> _state;
        readonly SensorTechnology _technology;
        readonly Sensor _config;

        public SensorSummary(Sensor portConfig, SensorTechnology technology)
        {
            _config = portConfig ?? throw new ArgumentNullException(nameof(portConfig));
            Label = portConfig.Name;
            Description = portConfig.Description;
            State = EntityHeader<SensorStates>.Create(SensorStates.Nominal);
            _technology = technology;
        }

        public EntityHeader<SensorStates> State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        double _value;
        public double Value
        {
            get => _value;
            set
            {
                Set(ref _value, value);
            }
        }

        string _display;
        public string Display
        {
            get => _display;
            set => Set(ref _display, value);
        }

        public string Label { get; }
        public string Description { get; }

        public Sensor Config => _config;
        public EntityHeader<SensorTechnology> Technology => EntityHeader<SensorTechnology>.Create(_technology);
    }
}
