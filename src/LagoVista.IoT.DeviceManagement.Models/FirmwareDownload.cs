// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fa4eddc13cd58967038666fe2e524a585962df4bd04561dd0d857dbf2dc3bf48
// IndexVersion: 0
// --- END CODE INDEX META ---
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
