using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupEntryRepo
    {
        
        Task AddDeviceToGroupAsync(EntityHeader deviceGroup, EntityHeader device);

        Task RemoveDeviceFromGroupAsync(string groupId, string deviceId);

        Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(string deviceGroupId);
    }
}
