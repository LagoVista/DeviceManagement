using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceArchiveRepo
    {
        Task AddArchiveAsync(DeviceRepository repo, DeviceArchive archiveEntry);
        Task<ListResponse<List<object>>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
        Task ClearDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId);
    }
}