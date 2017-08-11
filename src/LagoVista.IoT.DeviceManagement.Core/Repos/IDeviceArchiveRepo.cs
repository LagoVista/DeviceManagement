﻿using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceArchiveRepo
    {
        Task AddArchiveAsync(DeviceRepository repo, DeviceArchive archiveEntry);
        Task<ListResponse<List<string>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
    }
}