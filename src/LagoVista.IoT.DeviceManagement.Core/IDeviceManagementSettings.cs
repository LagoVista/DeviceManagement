using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public interface IDeviceManagementSettings
    {
        IConnectionSettings DeviceRepoStorage { get; set; }

        IConnectionSettings DefaultDeviceStorage { get; set; }
        IConnectionSettings DefaultDeviceTableStorage { get; set; }


        bool ShouldConsolidateCollections { get; }
    }
}
