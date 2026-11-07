using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceConnectionEventCassandraTests
    {
        [TestMethod]
        public void ActivityRecordPreservesDeviceConnectionFields()
        {
            var repository = new DeviceRepository { Id = "device-repository", Name = "Test Repository" };
            var connectionEvent = new DeviceConnectionEvent
            {
                DeviceId = "device-123",
                TimeStamp = "2026-11-07T06:00:00Z",
                FirmwareSKU = "firmware-sku",
                FirmwareRevision = "1.2.3",
                RSSI = -48.5,
                Reconnect = true
            };

            var record = DeviceConnectionActivityRecord.From(repository, connectionEvent);

            Assert.IsFalse(String.IsNullOrWhiteSpace(record.Id));
            Assert.AreEqual(repository.Id, record.OrganizationId);
            Assert.AreEqual(repository.Name, record.Organization);
            Assert.AreEqual(connectionEvent.DeviceId, record.DeviceUniqueId);
            Assert.AreEqual(new DateTime(2026, 11, 7, 6, 0, 0, DateTimeKind.Utc), record.CreationDate);

            var roundTrip = record.ToModel();
            Assert.AreEqual(connectionEvent.DeviceId, roundTrip.DeviceId);
            Assert.AreEqual(connectionEvent.TimeStamp, roundTrip.TimeStamp);
            Assert.AreEqual(connectionEvent.FirmwareSKU, roundTrip.FirmwareSKU);
            Assert.AreEqual(connectionEvent.FirmwareRevision, roundTrip.FirmwareRevision);
            Assert.AreEqual(connectionEvent.RSSI, roundTrip.RSSI);
            Assert.IsTrue(roundTrip.Reconnect);
        }

        [TestMethod]
        public async Task AddEventWritesToActivityStore()
        {
            var store = new Mock<IActivityRecordStore<DeviceConnectionActivityRecord>>();
            DeviceConnectionActivityRecord captured = null;
            store.Setup(s => s.InsertAsync(It.IsAny<DeviceConnectionActivityRecord>(), It.IsAny<CancellationToken>()))
                .Callback<DeviceConnectionActivityRecord, CancellationToken>((record, _) => captured = record)
                .Returns(Task.CompletedTask);
            var repo = new DeviceConnectionEventRepo(store.Object);

            var result = await repo.AddDeviceConnectionEventAsync(
                new DeviceRepository { Id = "device-repository", Name = "Test Repository" },
                new DeviceConnectionEvent { DeviceId = "device-123", Reconnect = true });

            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(captured);
            Assert.AreEqual("device-repository", captured.OrganizationId);
            Assert.AreEqual("device-123", captured.DeviceUniqueId);
            Assert.IsTrue(captured.Reconnect);
        }

        [TestMethod]
        public async Task GetEventsReturnsPagedHistoryModels()
        {
            var store = new Mock<IActivityRecordStore<DeviceConnectionActivityRecord>>();
            var records = new[]
            {
                new DeviceConnectionActivityRecord
                {
                    Id = "event-1",
                    OrganizationId = "device-repository",
                    DeviceUniqueId = "device-123",
                    DeviceId = "device-123",
                    Timestamp = "2026-11-07T06:00:00Z",
                    Reconnect = true
                }
            };
            store.Setup(s => s.QueryAsync(It.IsAny<HistoryQuery<DeviceConnectionActivityRecord>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StoragePageResult<DeviceConnectionActivityRecord>(records, "next-page"));
            var repo = new DeviceConnectionEventRepo(store.Object);

            var result = await repo.GetConnectionEventsForDeviceAsync(
                new DeviceRepository { Id = "device-repository" },
                "device-123",
                new ListRequest { PageSize = 25 });

            Assert.AreEqual(25, result.PageSize);
            Assert.IsTrue(result.HasMoreRecords);
            Assert.AreEqual(1, result.Model.Count());
            Assert.IsTrue(result.Model.Single().Reconnect);
        }
    }
}
