using LagoVista.CloudStorage.Utils;
using LagoVista.Core.Interfaces;
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
        public IConnectionSettings DefaultDeviceAccountStorage { get => TestConnections.DefaultDeviceAccountDb; set { } }
    }
}
