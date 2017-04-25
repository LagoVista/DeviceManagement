using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceManager
    {
        Task<InvokeResult> AddDeviceAsync(Device device, EntityHeader user, EntityHeader org);

        Task<InvokeResult> UpdateDeviceAsync(Device device, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeleteDeviceAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string orgId, int top, int take, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string locationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string status, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string configurationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByDeviceIdAsync(string id, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(string id, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByIdAsync(string id, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByIdAsync(string id);

        Task<Device> GetDeviceByDeviceIdAsync(string id);
    }
}
