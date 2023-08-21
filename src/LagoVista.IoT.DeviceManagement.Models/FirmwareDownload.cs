using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class FirmwareDownload
    {
        public long Size { get; set; }
        public int PercentComplete { get; set; }
        public string Message { get; set; }
        public byte[] Buffer { get; set; }
    }
}
