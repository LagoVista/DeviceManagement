using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceLogManager
    {
        Task AddEntryAsync(DeviceLog logEntry);

        Task<IEnumerable<DeviceLog>> GetForDateRangeAsync(string deviceId, int maxReturnCount = 100, String start = null, String end = null);
    }
}
