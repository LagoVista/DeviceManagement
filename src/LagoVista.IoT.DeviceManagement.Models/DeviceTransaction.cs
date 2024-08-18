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
