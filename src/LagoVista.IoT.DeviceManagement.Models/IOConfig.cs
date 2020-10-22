using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class IOConfig
    {
        public string LastUpdateFromDevice { get; set; }

        [JsonProperty("adc1config")]
        public byte ADC1Config { get; set; }

        [JsonProperty("adc2config")]
        public byte ADC2Config { get; set; }

        [JsonProperty("adc3config")]
        public byte ADC3Config { get; set; }

        [JsonProperty("adc4config")]
        public byte ADC4Config { get; set; }

        [JsonProperty("adc5config")] 
        public byte ADC5Config { get; set; }

        [JsonProperty("adc6config")] 
        public byte ADC6Config { get; set; }

        [JsonProperty("adc7config")] 
        public byte ADC7Config { get; set; }

        [JsonProperty("adc8config")] 
        public byte ADC8Config { get; set; }

        [JsonProperty("adc1scaler")]
        public float ADC1Scaler { get; set; }
        [JsonProperty("adc2scaler")]
        public float ADC2Scaler { get; set; }
        [JsonProperty("adc3scaler")]
        public float ADC3Scaler { get; set; }
        [JsonProperty("adc4scaler")]
        public float ADC4Scaler { get; set; }
        [JsonProperty("adc5scaler")]
        public float ADC5Scaler { get; set; }
        [JsonProperty("adc6scaler")]
        public float ADC6Scaler { get; set; }
        [JsonProperty("adc7scaler")]
        public float ADC7Scaler { get; set; }
        [JsonProperty("adc8scaler")]
        public float ADC8Scaler { get; set; }

        [JsonProperty("io1config")]
        public byte IO1Config { get; set; }
        [JsonProperty("io2config")]
        public byte IO2Config { get; set; }
        [JsonProperty("io3config")]
        public byte IO3Config { get; set; }
        [JsonProperty("io4config")]
        public byte IO4Config { get; set; }
        [JsonProperty("io5config")]
        public byte IO5Config { get; set; }
        [JsonProperty("io6config")]
        public byte IO6Config { get; set; }
        [JsonProperty("io7config")]
        public byte IO7Config { get; set; }
        [JsonProperty("io8config")]
        public byte IO8Config { get; set; }

        [JsonProperty("io1scaler")]
        public float IO1Scaler { get; set; }
        [JsonProperty("io2scaler")]
        public float IO2Scaler { get; set; }
        [JsonProperty("io3scaler")]
        public float IO3Scaler { get; set; }
        [JsonProperty("io4scaler")]
        public float IO4Scaler { get; set; }
        [JsonProperty("io5scaler")]
        public float IO5Scaler { get; set; }
        [JsonProperty("io6scaler")]
        public float IO6Scaler { get; set; }
        [JsonProperty("io7scaler")]
        public float IO7Scaler { get; set; }
        [JsonProperty("io8scaler")]
        public float IO8Scaler { get; set; }
    }
}
