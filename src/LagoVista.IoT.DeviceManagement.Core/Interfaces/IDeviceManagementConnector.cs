using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IDeviceManagementConnector
    {
        Task<InvokeResult> AddDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user);

        Task DeleteDeviceAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task UpdateDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user);

        Task DeleteDeviceByIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string instanceId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string instanceId, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByDeviceIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<bool> CheckIfDeviceIdInUse(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(string instanceId, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user);

    }
}
