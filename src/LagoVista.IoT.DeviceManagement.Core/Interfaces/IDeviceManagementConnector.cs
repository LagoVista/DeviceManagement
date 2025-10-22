// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c3b7031265bbfd65415b64ba1509919ac5059a41055b69dba18690c397fd2fd7
// IndexVersion: 0
// --- END CODE INDEX META ---
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

        Task<ListResponse<DeviceSummary>> GetDevicesForOrgIdAsync(string instanceId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(string instanceId, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByDeviceIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<bool> CheckIfDeviceIdInUse(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(string instanceId, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(string instanceId, string search, ListRequest listRequest, EntityHeader org, EntityHeader user);
        
        Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(string instanceId, string groupId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
