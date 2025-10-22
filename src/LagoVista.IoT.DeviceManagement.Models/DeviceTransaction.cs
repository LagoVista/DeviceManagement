// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 402b47fe478f61575d81aa4cc3da5d3d7bddb3b6e1d7164b84c495df10c8a0ff
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceTransaction
    {
        public int Id { get; set; }
        public string Timestamp { get; set; }
        public EntityHeader Device { get; set; }
        public EntityHeader Location { get; set; }

        public string Key { get; set; }
        public string Description { get; set; }
        public double? CreditAmount { get; set; }
        public double? DebitAmount { get; set; }
        public double AccountBalance { get; set; }
    }
}
