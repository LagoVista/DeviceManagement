# Firmware storage

Firmware persistence uses the shared CloudStorage capabilities. The firmware subsystem does not own provider-specific clients or connection strings.

| Responsibility | Repository/storage contract | Provider |
| --- | --- | --- |
| Firmware metadata and revisions | `FirmwareRepo : DocumentDBRepoBase<Firmware>` | Selected shared Entity Storage provider; DEV must select MongoDB |
| Download request and mutable status | `IApplicationDataStore<FirmwareDownloadRequestRecord>` | MongoDB Application Data |
| Firmware binaries | `ICloudFileStorageClient` | S3-compatible object storage (SeaweedFS) |

## Download request record

`FirmwareDownloadRequestRecord` is the CloudStorage persistence record for the existing public `FirmwareDownloadRequest` model. The public model and device/API contract remain unchanged.

- Mongo identity is the existing globally unique `DownloadId`.
- `Organization.Id` is the Application Data scope.
- Point lookup by download ID remains unscoped because the download ID is the existing device-facing capability.
- Device history queries always filter by organization, device repository and device ID.
- History is sorted by the existing ISO-8601 UTC `Timestamp` and paged through the Application Data continuation token.
- Mongo creates the canonical organization index plus indexes for device repository, device ID and timestamp.

The mutable status fields (requested, downloading, percentage, completed, failed, expired) remain one application record. They are not Cassandra activity events.

## Cutover decision

This DFU service has been offline for several years. Historical Azure Table download requests and old firmware metadata/binaries are not migrated. The new Mongo collections begin empty; newly created firmware requests and uploads use the current storage path. Existing Azure records are left untouched and are not read by the new repository.

Other DeviceManagement Azure Table repositories are outside this firmware cutover and remain unchanged. Cassandra continues to serve the existing device activity/history and current-status capabilities; it is not used for mutable firmware download request state.

## Runtime configuration

- Entity Storage uses the platform-wide `DefaultDocDBStorage:Provider` selection. DEV must select `Mongo` for firmware metadata to use the in-cluster Mongo provider.
- Application Data uses the existing `MongoDocumentStorage` and `ApplicationDataStorage` connection settings already used by other API Host features.
- Firmware binaries use the existing S3 object-storage settings and `firmware` container.
- Firmware download request persistence no longer consumes `DefaultTableStorage` credentials. API Host may still require Azure Table settings for unrelated legacy platform features.

No storage migration job or dual-read/dual-write compatibility layer is part of this cutover.
