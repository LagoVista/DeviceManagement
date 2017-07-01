using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceArchiveRepo
    {
        Task AddArchiveAsync(DeviceRepository repo, DeviceArchive archiveEntry);

        Task<String> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, int maxReturnCount, string start, string end);


    }
}