using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Interfaces
{
    public interface IDeviceStreamConnector
    {
        Task AddArchiveAsync(string instanceId, DeviceStreamRecord archiveEntry, EntityHeader org, EntityHeader user);
        Task<ListResponse<List<object>>> GetForDateRangeAsync(string streamId, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
