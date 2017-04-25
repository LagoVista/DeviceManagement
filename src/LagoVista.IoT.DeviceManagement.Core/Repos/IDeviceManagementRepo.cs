using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceManagementRepo
    {
        Task AddDeviceAsync(Device device);

        Task DeleteDeviceAsync(string id);

        Task UpdateDeviceAsync(Device device);

        Task DeleteDeviceByIdAsync(string deviceId);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string orgId, int top, int take);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string locationId, int top, int take);

        Task<Device> GetDeviceByDeviceIdAsync(string id);

        Task<bool> CheckIfDeviceIdInUse(string id, string orgid);

        Task<Device> GetDeviceByIdAsync(string id);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string status, int top, int take);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string configurationId, int top, int take);
    }
}