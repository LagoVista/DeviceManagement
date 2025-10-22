// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3540469b5db6b95e7ea515309e8093ef554b1976bbd0d8a608a1e44329af0097
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface ISilencedAlarmsRepo
    {
        Task AddSilencedAlarmAsync(DeviceRepository repo, SilencedAlarm silencedAlarm);
        Task<ListResponse<SilencedAlarm>> GetSilencedAlarmAsync(DeviceRepository repo, ListRequest listRequest, string deviceUniqueId);
    }
}
