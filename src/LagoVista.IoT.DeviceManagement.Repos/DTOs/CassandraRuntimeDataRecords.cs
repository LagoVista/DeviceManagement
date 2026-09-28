using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LagoVista.IoT.DeviceManagement.Repos.DTOs
{
    internal static class RuntimeDataRecordMapper
    {
        public static DateTime ParseTimestamp(string value)
        {
            if (!String.IsNullOrWhiteSpace(value) &&
                DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
                return parsed;
            return DateTime.UtcNow;
        }

        public static string NewId(string preferred = null) => String.IsNullOrWhiteSpace(preferred) ? Guid.NewGuid().ToId() : preferred;
    }

    public sealed class DeviceArchiveActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string MessageId { get; set; }
        public string DeviceConfigurationId { get; set; }
        public string Timestamp { get; set; }
        public string PEMMessageId { get; set; }
        public double DeviceConfigurationVersionId { get; set; }
        public string PropertiesJson { get; set; }

        public static DeviceArchiveActivityRecord From(DeviceRepository repo, DeviceArchive archive) => new DeviceArchiveActivityRecord
        {
            Id = RuntimeDataRecordMapper.NewId(archive.RowKey),
            OrganizationId = repo.Id,
            Organization = repo.Name,
            CreationDate = RuntimeDataRecordMapper.ParseTimestamp(archive.Timestamp),
            DeviceUniqueId = String.IsNullOrWhiteSpace(archive.PartitionKey) ? archive.DeviceId : archive.PartitionKey,
            DeviceId = archive.DeviceId,
            MessageId = archive.MessageId,
            DeviceConfigurationId = archive.DeviceConfigurationId,
            Timestamp = archive.Timestamp,
            PEMMessageId = archive.PEMMessageId,
            DeviceConfigurationVersionId = archive.DeviceConfigurationVersionId,
            PropertiesJson = JsonConvert.SerializeObject(archive.Properties ?? new Dictionary<string, object>())
        };

        public Dictionary<string, object> ToReportRow()
        {
            var row = String.IsNullOrWhiteSpace(PropertiesJson)
                ? new Dictionary<string, object>()
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(PropertiesJson) ?? new Dictionary<string, object>();
            row[nameof(DeviceArchive.RowKey)] = Id;
            row[nameof(DeviceArchive.PartitionKey)] = DeviceUniqueId;
            row[nameof(DeviceArchive.DeviceId)] = DeviceId;
            row[nameof(DeviceArchive.DeviceConfigurationId)] = DeviceConfigurationId;
            row[nameof(DeviceArchive.DeviceConfigurationVersionId)] = DeviceConfigurationVersionId;
            row[nameof(DeviceArchive.Timestamp)] = Timestamp ?? CreationDate.ToJSONString();
            row[nameof(DeviceArchive.PEMMessageId)] = PEMMessageId ?? String.Empty;
            return row;
        }
    }

    public sealed class SensorDataArchiveActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public double? ADCSensor1 { get; set; }
        public double? ADCSensor2 { get; set; }
        public double? ADCSensor3 { get; set; }
        public double? ADCSensor4 { get; set; }
        public double? ADCSensor5 { get; set; }
        public double? ADCSensor6 { get; set; }
        public double? ADCSensor7 { get; set; }
        public double? ADCSensor8 { get; set; }
        public double? IoSensor1 { get; set; }
        public double? IoSensor2 { get; set; }
        public double? IoSensor3 { get; set; }
        public double? IoSensor4 { get; set; }
        public double? IoSensor5 { get; set; }
        public double? IoSensor6 { get; set; }
        public double? IoSensor7 { get; set; }
        public double? IoSensor8 { get; set; }

        public static SensorDataArchiveActivityRecord From(DeviceRepository repo, SensorDataArchive archive) => new SensorDataArchiveActivityRecord
        {
            Id = RuntimeDataRecordMapper.NewId(archive.RowKey), OrganizationId = repo.Id, Organization = repo.Name,
            CreationDate = RuntimeDataRecordMapper.ParseTimestamp(archive.Timestamp),
            DeviceUniqueId = String.IsNullOrWhiteSpace(archive.PartitionKey) ? archive.DeviceId : archive.PartitionKey,
            DeviceId = archive.DeviceId, Timestamp = archive.Timestamp,
            ADCSensor1 = archive.ADCSensor1, ADCSensor2 = archive.ADCSensor2, ADCSensor3 = archive.ADCSensor3, ADCSensor4 = archive.ADCSensor4,
            ADCSensor5 = archive.ADCSensor5, ADCSensor6 = archive.ADCSensor6, ADCSensor7 = archive.ADCSensor7, ADCSensor8 = archive.ADCSensor8,
            IoSensor1 = archive.IoSensor1, IoSensor2 = archive.IoSensor2, IoSensor3 = archive.IoSensor3, IoSensor4 = archive.IoSensor4,
            IoSensor5 = archive.IoSensor5, IoSensor6 = archive.IoSensor6, IoSensor7 = archive.IoSensor7, IoSensor8 = archive.IoSensor8
        };

        public SensorDataArchive ToModel() => new SensorDataArchive
        {
            RowKey = Id, PartitionKey = DeviceUniqueId, DeviceId = DeviceId, Timestamp = Timestamp,
            ADCSensor1 = ADCSensor1, ADCSensor2 = ADCSensor2, ADCSensor3 = ADCSensor3, ADCSensor4 = ADCSensor4,
            ADCSensor5 = ADCSensor5, ADCSensor6 = ADCSensor6, ADCSensor7 = ADCSensor7, ADCSensor8 = ADCSensor8,
            IoSensor1 = IoSensor1, IoSensor2 = IoSensor2, IoSensor3 = IoSensor3, IoSensor4 = IoSensor4,
            IoSensor5 = IoSensor5, IoSensor6 = IoSensor6, IoSensor7 = IoSensor7, IoSensor8 = IoSensor8
        };
    }

    public sealed class DeviceExceptionActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceRepositoryId { get; set; }
        public string ErrorCode { get; set; }
        public string Details { get; set; }
        public string AdditionalDetails { get; set; }
        public string Timestamp { get; set; }
        public bool Cleared { get; set; }
        public string Event { get; set; }

        public static DeviceExceptionActivityRecord From(DeviceRepository repo, DeviceException exception, bool cleared) => new DeviceExceptionActivityRecord
        {
            Id = RuntimeDataRecordMapper.NewId(exception.Id), OrganizationId = repo.Id, Organization = repo.Name,
            CreationDate = RuntimeDataRecordMapper.ParseTimestamp(exception.Timestamp), DeviceUniqueId = exception.DeviceUniqueId,
            DeviceId = exception.DeviceId, DeviceRepositoryId = exception.DeviceRepositoryId, ErrorCode = exception.ErrorCode,
            Details = exception.Details, AdditionalDetails = exception.AdditionalDetails == null ? String.Empty : String.Join(",", exception.AdditionalDetails),
            Timestamp = exception.Timestamp, Cleared = cleared, Event = exception.Event
        };

        public DeviceException ToModel() => new DeviceException
        {
            Id = Id, DeviceUniqueId = DeviceUniqueId, DeviceId = DeviceId, DeviceRepositoryId = DeviceRepositoryId,
            ErrorCode = ErrorCode, Details = Details,
            AdditionalDetails = String.IsNullOrWhiteSpace(AdditionalDetails) ? new List<string>() : AdditionalDetails.Split(',').ToList(),
            Timestamp = Timestamp, Cleared = Cleared, Event = Event
        };
    }

    public sealed class DeviceStatusHistoryActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string WatchdogCheckPoint { get; set; }
        public int WatchdogTimeoutSeconds { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Details { get; set; }
        public bool SilenceAlarm { get; set; }

        public static DeviceStatusHistoryActivityRecord From(DeviceRepository repo, DeviceStatus status) => new DeviceStatusHistoryActivityRecord
        {
            Id = Guid.NewGuid().ToId(), OrganizationId = repo.Id, Organization = repo.Name,
            CreationDate = RuntimeDataRecordMapper.ParseTimestamp(status.Timestamp ?? status.LastContact),
            DeviceUniqueId = status.DeviceUniqueId, DeviceId = status.DeviceId, Timestamp = status.Timestamp,
            LastContact = status.LastContact, LastNotified = status.LastNotified, WatchdogCheckPoint = status.WatchdogCheckPoint,
            WatchdogTimeoutSeconds = status.WatchdogTimeoutSeconds, PreviousStatus = status.PreviousStatus,
            CurrentStatus = status.CurrentStatus, Details = status.Details, SilenceAlarm = status.SilenceAlarm
        };

        public DeviceStatus ToModel() => new DeviceStatus
        {
            DeviceUniqueId = DeviceUniqueId, DeviceId = DeviceId, Timestamp = Timestamp, LastContact = LastContact,
            LastNotified = LastNotified, WatchdogCheckPoint = WatchdogCheckPoint, WatchdogTimeoutSeconds = WatchdogTimeoutSeconds,
            PreviousStatus = PreviousStatus, CurrentStatus = CurrentStatus, Details = Details, SilenceAlarm = SilenceAlarm
        };
    }

    public sealed class DeviceCurrentStatusRecord : IOperationalDataRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string WatchdogCheckPoint { get; set; }
        public int WatchdogTimeoutSeconds { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Details { get; set; }
        public bool SilenceAlarm { get; set; }

        public static DeviceCurrentStatusRecord From(DeviceRepository repo, DeviceStatus status) => new DeviceCurrentStatusRecord
        {
            Id = status.DeviceUniqueId, OrganizationId = repo.Id, DeviceId = status.DeviceId, Timestamp = status.Timestamp,
            LastContact = status.LastContact, LastNotified = status.LastNotified, WatchdogCheckPoint = status.WatchdogCheckPoint,
            WatchdogTimeoutSeconds = status.WatchdogTimeoutSeconds, PreviousStatus = status.PreviousStatus,
            CurrentStatus = status.CurrentStatus, Details = status.Details, SilenceAlarm = status.SilenceAlarm
        };

        public DeviceStatus ToModel() => new DeviceStatus
        {
            DeviceUniqueId = Id, DeviceId = DeviceId, Timestamp = Timestamp, LastContact = LastContact, LastNotified = LastNotified,
            WatchdogCheckPoint = WatchdogCheckPoint, WatchdogTimeoutSeconds = WatchdogTimeoutSeconds, PreviousStatus = PreviousStatus,
            CurrentStatus = CurrentStatus, Details = Details, SilenceAlarm = SilenceAlarm
        };
    }
}