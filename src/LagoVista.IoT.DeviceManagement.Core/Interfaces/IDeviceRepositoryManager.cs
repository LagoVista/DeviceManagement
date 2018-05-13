using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceRepositoryManagerRemote
    {
        Task<DeviceRepository> GetDeviceRepositoryAsync(string repoId, EntityHeader org, EntityHeader user);

        Task<DeviceRepository> GetDeviceRepositoryWithSecretsAsync(string repoId, EntityHeader org, EntityHeader user);
    }

    public interface IDeviceRepositoryManager : IDeviceRepositoryManagerRemote
    {
        Task<InvokeResult> AddDeviceRepositoryAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string deviceRepoId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeviceRepositoryAsync(String deviceRepoId, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);
        Task<bool> QueryKeyInUserAsync(string key, EntityHeader org);
    }
}