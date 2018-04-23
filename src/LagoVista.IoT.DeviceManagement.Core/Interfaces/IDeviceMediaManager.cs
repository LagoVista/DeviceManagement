using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceMediaManager
    {
        Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, EntityHeader org, EntityHeader user, ListRequest request);

        Task<MediaItemResponse> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string ItemId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddMediaItemAsync(DeviceRepository repo, string deviceId, Stream stream, string contentType, EntityHeader org, EntityHeader user);
    }
}