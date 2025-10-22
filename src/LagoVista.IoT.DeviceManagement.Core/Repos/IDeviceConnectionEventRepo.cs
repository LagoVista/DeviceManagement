// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9cfff2030bc8da7a222466f677e355fd68bde074b8b10189be30b1b81b5d551c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceConnectionEventRepo
    {
        Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, String deviceId, ListRequest request);
    }
}
