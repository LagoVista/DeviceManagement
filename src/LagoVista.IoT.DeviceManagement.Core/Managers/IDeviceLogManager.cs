using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceLogManager
    {
        Task AddEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry);

        Task<IEnumerable<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, int maxReturnCount = 100, String start = null, String end = null);
    }
}
