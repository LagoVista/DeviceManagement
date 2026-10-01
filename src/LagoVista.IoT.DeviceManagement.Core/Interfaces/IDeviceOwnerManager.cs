using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Users;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceOwnerManager
    {
        Task<InvokeResult> CreateOwnerAsync(DeviceOwnerUser owner, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceOwnerUser>> GetOwnerByIdAsync(string ownerId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceOwnerUser>> GetOwnerByPhoneAsync(string phoneNumber, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceOwnerUser>> GetOwnerByEmailAsync(string email, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceOwnerUser>> GetOwnerByNameAsync(string userName, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateOwnerAsync(DeviceOwnerUser owner, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteOwnerAsync(string ownerId, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceOwnerUserSummary>> GetOwnersAsync(ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceOwnerUser>> GetOwnersForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceOwnerDevices[]>> GetDevicesForOwnerAsync(string ownerId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> AssignOwnerToDeviceAsync(DeviceRepository deviceRepo, string deviceId, string ownerId, bool replaceExisting, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RemoveOwnerFromDeviceAsync(DeviceRepository deviceRepo, string deviceId, string ownerId, EntityHeader org, EntityHeader user);
    }
}
