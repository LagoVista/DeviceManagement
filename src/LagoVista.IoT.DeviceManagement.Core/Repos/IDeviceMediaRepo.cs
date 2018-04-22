using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceMediaRepo
    {
        Task<DeviceMedia> GetMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);
        Task<ListResponse<DeviceMedia>> GetMediaItemsForDeviceAsync(DeviceRepository repo, string deviceId, ListRequest request);
        Task DeleteMediaItemAsync(DeviceRepository repo, string deviceId, string itemId);
    }
}
