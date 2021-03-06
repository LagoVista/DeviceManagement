﻿using LagoVista.IoT.DeviceManagement.Core.Models;
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

        Task<IEnumerable<DeviceRepositorySummary>> GetDeviceRepositoriesForOrgAsync(string orgid);

        Task DeleteAsync(String repoId);

        Task<bool> QueryRepoKeyInUseAsync(string key, string orgId);
    }
}
