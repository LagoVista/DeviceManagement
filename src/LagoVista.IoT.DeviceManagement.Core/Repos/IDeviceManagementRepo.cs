using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceManagementRepo
    {
        Task AddDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceAsync(DeviceRepository repo, string id);

        Task UpdateDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository repo, string orgId, int top, int take);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, int top, int take);

        Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id);

        Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string id, string orgid);

        Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, int top, int take);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, int top, int take);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, int top, int take);
    }
}