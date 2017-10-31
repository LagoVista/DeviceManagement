using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceManagementRepo
    {
        Task<InvokeResult> AddDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceAsync(DeviceRepository repo, string id);

        Task UpdateDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository repo, string orgId, ListRequest listRequest);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, ListRequest listRequest);

        Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id);

        Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string id, string orgid);

        Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, ListRequest listRequest);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, ListRequest listRequest);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, ListRequest listRequest);
    }

}