// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b45233967832d16956469c036d9206c6ce827d5ba3be74574ca849780ed658db
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceArchiveManagerRemote
    {
        Task<InvokeResult> AddArchiveAsync(DeviceRepository deviceRepo, DeviceArchive logEntry, EntityHeader org, EntityHeader user);
    }

    public interface IDeviceArchiveManager : IDeviceArchiveManagerRemote
    {
        Task<ListResponse<List<object>>> GetDeviceArchivesAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
