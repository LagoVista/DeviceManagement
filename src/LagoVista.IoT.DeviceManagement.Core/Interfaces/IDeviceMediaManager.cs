// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9fef5dbacc04ccfb692cde5fe501aa7a53b30975676a7776a2a509ab0e6d71a8
// IndexVersion: 0
// --- END CODE INDEX META ---
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