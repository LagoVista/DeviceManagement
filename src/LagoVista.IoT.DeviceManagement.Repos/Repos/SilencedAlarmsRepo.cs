// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2fc3820bcf7c502cbefc03f03689d015cac310d44760e688dc50f6f422c293bd
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class SilencedAlarmsRepo : TableStorageBase<SilancedAlarmDTO>, ISilencedAlarmsRepo
    {
        public SilencedAlarmsRepo(IAdminLogger logger) : base(logger)
        {
        }

        public async Task AddSilencedAlarmAsync(DeviceRepository repo, SilencedAlarm silencedAlarm)
        {
            SetTableName(repo.GetSilencedAlarmsStorageName());
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            await InsertAsync(SilancedAlarmDTO.CreateDTO(silencedAlarm));
        }

        public async Task<ListResponse<SilencedAlarm>> GetSilencedAlarmAsync(DeviceRepository repo, ListRequest listRequest, string deviceUniqueId)
        {
            SetTableName(repo.GetSilencedAlarmsStorageName());
            SetConnection(repo.DeviceArchiveStorageSettings.AccountId, repo.DeviceArchiveStorageSettings.AccessKey);
            var result = await this.GetPagedResultsAsync(deviceUniqueId, listRequest);
            return ListResponse<SilencedAlarm>.Create(listRequest, result.Model.Select(sa => sa.ToSilencedAlarm()));
        }
    }

    public class SilancedAlarmDTO : TableStorageEntity
    {
        public bool Disabled { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string RepoName { get; set; }
        public string RepoId { get; set; }
        public string Details { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public string Timestamp { get; set; }


        public SilencedAlarm ToSilencedAlarm()
        {
            return new SilencedAlarm()
            {
                Details = Details,
                User = EntityHeader.Create(UserId, User),
                Device = EntityHeader.Create(DeviceUniqueId, DeviceId, DeviceName),
                DeviceRepo = EntityHeader.Create(RepoId, RepoName),
                Timestamp = Timestamp,
                Disabled = Disabled,
            };
        }

        public static SilancedAlarmDTO CreateDTO(SilencedAlarm alarm)
        {
            return new SilancedAlarmDTO()
            {
                Disabled = alarm.Disabled,
                DeviceUniqueId = alarm.Device.Key,
                DeviceId = alarm.Device.Id,
                DeviceName = alarm.Device.Text,
                Details = alarm.Details,
                Timestamp = alarm.Timestamp,
                RowKey = DateTime.UtcNow.ToInverseTicksRowKey(),
                PartitionKey = alarm.Device.Id,
                User = alarm.User.Text,
                UserId = alarm.User.Id,
                RepoId = alarm.DeviceRepo.Id,
                RepoName = alarm.DeviceRepo.Text
            };
        }
    }
}
