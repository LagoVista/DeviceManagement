using LagoVista.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public class DataStreamRecord : TableStorageEntity
    {
        public DataStreamRecord()
        {
            Data = new Dictionary<string, object>();
        }

        public string StreamId { get; set; }

        public string Timestamp { get; set; }

        public string DeviceId { get; set; }

        [JsonExtensionData]
        public Dictionary<String, object> Data { get; set; }

    }
}
