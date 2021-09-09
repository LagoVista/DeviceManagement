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

        private void Evaluate(double value)
        {
            SensorStates state;

            if (_config.AttributeType.Value == SensorValueType.Boolean)
            {
                if (value == 0)
                {
                    state = SensorStates.Off;
                    Display = "Off";
                }
                else
                {
                    state = SensorStates.On;
                    Display = "On";
                }
            }
            else if(_config.AttributeType.Value == SensorValueType.Number)
            {
                if (EntityHeader.IsNullOrEmpty(_config.UnitSet))
                {
                    Display = $"{Value}";
                }
                else
                {
                    Display = $"{Value}";
                }

                var dblValue = Convert.ToDouble(value);
                var range = _config.HighThreshold - _config.LowThreshold;
                var warningThreshold = range * 0.20;

                Set(ref _value, value);
                if (dblValue < _config.LowThreshold ||
                    dblValue > _config.HighThreshold)
                {
                    state = SensorStates.Error;
                }
                else if (dblValue < (_config.LowThreshold + warningThreshold) ||
                         dblValue > _config.HighThreshold - warningThreshold)
                {
                    state = SensorStates.Warning;
                }
                else
                {
                    state = SensorStates.Nominal;

                }
            }
            else
            {
                state = SensorStates.Nominal;
            }

            State = EntityHeader<SensorStates>.Create(state);
            _config.State = EntityHeader<SensorStates>.Create(state);
        }

        double _value;
        public double Value
        {
            get => _value;
            set
            {
                Set(ref _value, value);
                Evaluate(value);
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
