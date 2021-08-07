﻿using Newtonsoft.Json;
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
                AdcConfigs[idx] = new Sensor() { PortIndex = idx };
                IoConfigs[idx] = new Sensor() { PortIndex = idx };
            }

            BluetoothValues = new List<double>();
        }        

        public string LastUpdateFromDevice { get; set; }

        public Sensor[] AdcConfigs { get; set; } = new Sensor[8];
        public Sensor[] IoConfigs { get; set; } = new Sensor[8];

        public List<Sensor> BluetoothConfigs { get; set; }

        public double[] AdcValues { get; set; } = new double[8];
        public double[] IoValues { get; set; } = new double[8];

        public List<double> BluetoothValues { get; set; }
    }
}
