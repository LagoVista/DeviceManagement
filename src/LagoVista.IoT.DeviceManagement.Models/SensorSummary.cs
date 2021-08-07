using LagoVista.Core.Models;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public enum SensorStates
    {
        Offline,
        Nominal,
        Warning,
        Error,
        Off,
        On,
    }

    public class SensorSummary : ModelBase
    {
        SensorStates _state;
        readonly SensorTechnology _technology;
        readonly Sensor _config;

        public SensorSummary(Sensor portConfig, SensorTechnology technology)
        {
            _config = portConfig ?? throw new ArgumentNullException(nameof(portConfig));
            Label = portConfig.Name;
            Description = portConfig.Description;
            State = SensorStates.Nominal;
            _technology = technology;
        }

        public SensorStates State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        private void Evaluate(double value)
        {
            if (_config.AttributeType.Value == DeviceAdmin.Models.ParameterTypes.TrueFalse)
            {
                if (value == 0)
                {
                    State = SensorStates.Off;
                    Display = "Off";
                }
                else
                {
                    State = SensorStates.On;
                    Display = "On";
                }
            }
            else
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
                var range = _config.HighTheshold - _config.LowThreshold;
                var warningThreshold = range * 0.20;

                Set(ref _value, value);
                if (dblValue < _config.LowThreshold ||
                    dblValue > _config.HighTheshold)
                {
                    State = SensorStates.Error;
                }
                else if (dblValue < (_config.LowThreshold + warningThreshold) ||
                         dblValue > _config.HighTheshold - warningThreshold)
                {
                    State = SensorStates.Warning;
                }
                else
                {
                    State = SensorStates.Nominal;
                    
                }
            }

            _config.State = State;
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
        public SensorTechnology Technology => _technology;
    }
}
