using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceMedia
    {
        public string ItemId { get; set; }
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
    }
}
