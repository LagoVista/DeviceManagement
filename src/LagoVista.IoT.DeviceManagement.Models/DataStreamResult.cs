using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public class DataStreamResult
    {
        public DataStreamResult()
        {
            Fields = new Dictionary<string, object>();
        }

        public string Timestamp { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
