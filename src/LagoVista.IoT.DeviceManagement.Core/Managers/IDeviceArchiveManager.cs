using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceArchiveManager
    {
        Task AddArchiveAsync(DeviceArchive logEntry);

        Task<IEnumerable<DeviceArchive>> GetForDateRangeAsync(string deviceId, int maxReturnCount = 100, string start = null, string end = null);
    }
}
