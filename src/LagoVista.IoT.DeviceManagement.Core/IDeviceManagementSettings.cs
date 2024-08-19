using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public interface IDeviceManagementSettings
    {
        IConnectionSettings DeviceRepoStorage { get; set; }

        IConnectionSettings DefaultDeviceStorage { get; set; }
        IConnectionSettings DefaultDeviceTableStorage { get; set; }

        ConnectionSettings DefaultDeviceAccountStorage { get; set; }

        bool ShouldConsolidateCollections { get; }
    }
}
