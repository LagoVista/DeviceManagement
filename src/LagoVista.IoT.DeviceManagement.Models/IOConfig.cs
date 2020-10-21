using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class IOConfig
    {
        public string LastUpdateFromDevice { get; set; }

        public byte ADC1Config { get; set; }
        public byte ADC2Config { get; set; }
        public byte ADC3Config { get; set; }
        public byte ADC4Config { get; set; }
        public byte ADC5Config { get; set; }
        public byte ADC6Config { get; set; }
        public byte ADC7Config { get; set; }
        public byte ADC8Config { get; set; }

        public float ADC1Scaler { get; set; }
        public float ADC2Scaler { get; set; }
        public float ADC3Scaler { get; set; }
        public float ADC4Scaler { get; set; }
        public float ADC5Scaler { get; set; }
        public float ADC6Scaler { get; set; }
        public float ADC7Scaler { get; set; }
        public float ADC8Scaler { get; set; }

        public byte IO1Config { get; set; }
        public byte IO2Config { get; set; }
        public byte IO3Config { get; set; }
        public byte IO4Config { get; set; }
        public byte IO5Config { get; set; }
        public byte IO6Config { get; set; }
        public byte IO7Config { get; set; }
        public byte IO8Config { get; set; }

        public float IO1Scaler { get; set; }
        public float IO2Scaler { get; set; }
        public float IO3Scaler { get; set; }
        public float IO4Scaler { get; set; }
        public float IO5Scaler { get; set; }
        public float IO6Scaler { get; set; }
        public float IO7Scaler { get; set; }
        public float IO8Scaler { get; set; }
    }
}
