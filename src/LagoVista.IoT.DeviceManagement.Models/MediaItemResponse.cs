using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class MediaItemResponse
    {
        public byte[] ImageBytes { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }
    }
}
