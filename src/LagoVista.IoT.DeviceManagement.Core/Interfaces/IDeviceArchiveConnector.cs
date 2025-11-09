// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bc86126393431451e3f6575427887d0236e903a9f15d2d9202df7eeec9893e04
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IDeviceArchiveConnector
    {
        Task AddArchiveAsync(string instanceId, DeviceArchive archiveEntry, EntityHeader org, EntityHeader user);
        Task<ListResponse<List<object>>> GetForDateRangeAsync(string instanceId, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
