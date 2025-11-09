// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: efe6d6868dad3e595cb76899ef527ff46a466fda44d1c92420ce09068c031e6f
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceRepositoryRepo
    {
        Task AddDeviceRepositoryAsync(DeviceRepository deviceRepo);

        Task UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo);

        Task<DeviceRepository> GetDeviceRepositoryAsync(string repoId);
        Task<DeviceRepository> GetDeviceRepositoryForInstanceAsync(string instanceId);

        Task<ListResponse<DeviceRepositorySummary>> GetDeviceRepositoriesForOrgAsync(string orgid, ListRequest listRequest);
        Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeviceRepositoriesForOrgAsync(string orgid, ListRequest listRequest);

        Task DeleteAsync(String repoId);

        Task<bool> QueryRepoKeyInUseAsync(string key, string orgId);
    }
}
