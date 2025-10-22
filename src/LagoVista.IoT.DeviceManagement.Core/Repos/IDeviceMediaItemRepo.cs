// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3a6a512ba36addcbda123b14a4ab35ce57d97135685bf630ea94b5eef0bd5f1b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    /* These are used to contain the actual media items that contain the meta data for the media */
    public interface IDeviceMediaItemRepoRemote
    {
        Task<InvokeResult> StoreMediaItemAsync(DeviceRepository repo, DeviceMedia media);
    }

    public interface IDeviceMediaItemRepo : IDeviceMediaItemRepoRemote
    {
        Task<DeviceMedia> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);
        Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, ListRequest request);
        Task DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);
    }
}
