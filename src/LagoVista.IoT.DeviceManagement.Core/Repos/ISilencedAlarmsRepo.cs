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
