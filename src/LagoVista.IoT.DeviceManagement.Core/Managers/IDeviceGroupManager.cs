using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceGroupManager
    {
        Task<InvokeResult> AddDeviceGroupAsync(DeviceGroup deviceGroup, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceGroupAsync(DeviceGroup deviceGroup, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(string orgId, EntityHeader user);

        Task<DeviceGroup> GetDeviceGroupAsync(string groupId, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckDeviceGroupInUseAsync(string groupId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddDeviceToGroupAsync(string deviceGroupId, string deviceId, EntityHeader org,  EntityHeader user);

        Task<InvokeResult> RemoveDeviceFromGroupAsync(string deviceGroupId, string deviceId, EntityHeader org, EntityHeader user);

        Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(string deviceGroupId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeleteDeviceGroupAsync(string deviceGroupId, EntityHeader org, EntityHeader user);

        Task<bool> QueryKeyInUseAsync(string key, EntityHeader org);
    }
}
