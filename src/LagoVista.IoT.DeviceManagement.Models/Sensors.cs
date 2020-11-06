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
        }        

        public string LastUpdateFromDevice { get; set; }

        public PortConfig[] AdcConfigs { get; set; } = new PortConfig[8];
        public PortConfig[] IoConfigs { get; set; } = new PortConfig[8];
    }
}
