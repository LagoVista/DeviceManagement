﻿using LagoVista.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class PortConfig    
    {
        public PortConfig()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }
        public string Address { get; set; }
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
        public bool InTolerance { get; set; }
        public bool AlertsEnabled { get; set; }
    }
}
