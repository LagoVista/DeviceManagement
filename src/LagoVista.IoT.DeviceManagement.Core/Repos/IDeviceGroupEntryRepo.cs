using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupEntryRepo
    {
        
        Task AddDeviceToGroupAsync(DeviceRepository deviceRepo, EntityHeader deviceGroup, EntityHeader device);

        Task RemoveDeviceFromGroupAsync(DeviceRepository deviceRepo, string groupId, string deviceId);

        Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(DeviceRepository deviceRepo, string deviceGroupId);
    }
}
