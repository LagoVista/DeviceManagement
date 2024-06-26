﻿using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceExceptionRepo
    {
        Task AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException exception);
        Task AddDeviceExceptionClearedAsync(DeviceRepository deviceRepo, DeviceException exception);
        Task<ListResponse<DeviceException>> GetDeviceExceptionsAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
        Task ClearDeviceExceptionsAsync(DeviceRepository deviceRepo, string id);
    }
}
