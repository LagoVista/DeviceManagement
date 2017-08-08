using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceArchiveManager
    {
        Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceArchive>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
