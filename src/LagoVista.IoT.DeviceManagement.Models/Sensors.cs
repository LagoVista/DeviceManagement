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
                AdcConfigs[idx] = new PortConfig() { SensorIndex = idx };
                IoConfigs[idx] = new PortConfig() { SensorIndex = idx };
            }

            BluetoothValues = new List<double>();
        }        

        public string LastUpdateFromDevice { get; set; }

        public PortConfig[] AdcConfigs { get; set; } = new PortConfig[8];
        public PortConfig[] IoConfigs { get; set; } = new PortConfig[8];

        public List<PortConfig> BluetoothConfigs { get; set; }

        public double[] AdcValues { get; set; } = new double[8];
        public double[] IoValues { get; set; } = new double[8];

        public List<double> BluetoothValues { get; set; }
    }
}
