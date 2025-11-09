// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3ed157eff2fa5228926b128b9f5dcdd5c71d7722732f952a6fb1bc509239a3e0
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceGroupManager
    {
        Task<InvokeResult> AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId, EntityHeader user);

        Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user);
        Task<DeviceGroup> GetDeviceGroupByKeyAsync(DeviceRepository deviceRepo, string key, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckDeviceGroupInUseAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<DeviceGroupEntry>> AddDeviceToGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, string deviceId, EntityHeader org,  EntityHeader user);

        Task<InvokeResult> RemoveDeviceFromGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, string deviceId, EntityHeader org, EntityHeader user);

        Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user);

        Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, EntityHeader org);
        Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(DeviceRepository repo, string groupId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
