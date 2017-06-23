using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public interface IDeviceManagementSettings
    {
        IConnectionSettings DeviceManagementDocDbStorage { get; set; }
        IConnectionSettings DeviceManagementTableStorage { get; set; }

        IConnectionSettings PEMStorage { get; set; }

        IConnectionSettings DeviceManagementArchiveTableStorage { get; set; }
        IConnectionSettings DeviceManagementLogTableStorage { get; set; }

        bool ShouldConsolidateCollections { get; }
    }
}
