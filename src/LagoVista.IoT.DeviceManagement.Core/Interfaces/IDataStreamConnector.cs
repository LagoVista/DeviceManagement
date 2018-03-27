using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceStreamConnector
    {
        Task AddArchiveAsync(string instanceId, DataStreamRecord archiveEntry, EntityHeader org, EntityHeader user);
        Task<ListResponse<List<object>>> GetForDateRangeAsync(string streamId, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
