// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b1864f291657298030a8a879a71e7b07b15217704773d98ed5de8aef809fb4ad
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Utils;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests
{
    public class DMSettings : IDeviceManagementSettings
    {
        public IConnectionSettings DeviceRepoStorage { get  => TestConnections.DevDocDB; set { } }
        public IConnectionSettings DefaultDeviceStorage { get => TestConnections.DevDocDB; set  { } }
        public IConnectionSettings DefaultDeviceTableStorage { get => TestConnections.DevTableStorageDB; set { }  }

        public bool ShouldConsolidateCollections { get => true; }
        public ConnectionSettings DefaultDeviceAccountStorage { get => TestConnections.DefaultDeviceAccountDb; set { } }
    }
}
