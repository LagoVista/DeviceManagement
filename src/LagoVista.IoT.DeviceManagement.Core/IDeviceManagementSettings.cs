// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8e5d2bc770e603529da24e7ac1ed9b03608368d5cc7dccbcd627d096cfa363d6
// IndexVersion: 0
// --- END CODE INDEX META ---
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
