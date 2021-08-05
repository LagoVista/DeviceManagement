using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class Sensors
    {
        public Sensors()
        {
            for (int idx = 0; idx < 8; ++idx)
            {
                AdcConfigs[idx] = new SensorConfig() { SensorIndex = idx };
                IoConfigs[idx] = new SensorConfig() { SensorIndex = idx };
            }

            BluetoothValues = new List<double>();
        }        

        public string LastUpdateFromDevice { get; set; }

        public SensorConfig[] AdcConfigs { get; set; } = new SensorConfig[8];
        public SensorConfig[] IoConfigs { get; set; } = new SensorConfig[8];

        public List<SensorConfig> BluetoothConfigs { get; set; }

        public double[] AdcValues { get; set; } = new double[8];
        public double[] IoValues { get; set; } = new double[8];

        public List<double> BluetoothValues { get; set; }
    }
}
