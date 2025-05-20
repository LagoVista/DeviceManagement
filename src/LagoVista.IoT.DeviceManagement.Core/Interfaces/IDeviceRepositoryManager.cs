using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceRepositoryManagerRemote
    {
        Task<DeviceRepository> GetDeviceRepositoryAsync(string repoId, EntityHeader org, EntityHeader user);

        Task<DeviceRepository> GetDeviceRepositoryWithSecretsAsync(string repoId, EntityHeader org, EntityHeader user, string pin = null, bool anonymous = false);
    }

    public interface IDeviceRepositoryManager : IDeviceRepositoryManagerRemote
    {
        Task<DeviceRepository> GetDeviceRepositoryForInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> AddDeviceRepositoryAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string deviceRepoId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeviceRepositoryAsync(String deviceRepoId, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user);
        Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeploymentHostsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user);
        Task<bool> QueryKeyInUserAsync(string key, EntityHeader org);
        Task<InvokeResult<string>> GetRepoLogoAsync(string logId);
        Task<InvokeResult<BasicTheme>> GetBasicThemeForRepoAsync(string orgid, string id);
    }
}