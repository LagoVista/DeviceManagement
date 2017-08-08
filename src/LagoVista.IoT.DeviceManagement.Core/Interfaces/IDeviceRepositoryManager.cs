using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceRepositoryManager
    {
        Task<InvokeResult> AddDeviceRepositoryAsync(DeviceRepository host, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository host, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeviceRepositoryAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<DeviceRepository> GetDeviceRepositoryAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);
        Task<bool> QueryKeyInUserAsync(string key, EntityHeader org);
    }
}