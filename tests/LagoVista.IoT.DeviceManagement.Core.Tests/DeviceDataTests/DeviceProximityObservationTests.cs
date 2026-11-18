using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceProximityObservationTests
    {
        private static DeviceProximityObservation Observation(string id) => new DeviceProximityObservation
        {
            OrganizationId = "org-1",
            Organization = "Organization 1",
            DeviceRepoId = "repo-1",
            DeviceUniqueId = "device-1",
            DeviceName = "Button",
            DeviceId = "physical-1",
            DeviceType = "Single Button",
            DeviceTypeId = "type-1",
            ObservedDeviceId = id,
            TimeStamp = new DateTime(2026, 11, 18, 15, 0, 0, DateTimeKind.Utc),
            Rssi = -62.5,
            DistanceEstimate = 2.34,
            SourceMessageType = "beacons"
        };

        [TestMethod]
        public void RecordRoundTripsNormalizedFields()
        {
            var observation = Observation("Beacon01");
            var record = DeviceProximityObservationActivityRecord.From(observation);
            var roundTrip = record.ToModel();
            Assert.IsFalse(String.IsNullOrWhiteSpace(roundTrip.Id));
            Assert.AreEqual(observation.OrganizationId, roundTrip.OrganizationId);
            Assert.AreEqual(observation.DeviceRepoId, roundTrip.DeviceRepoId);
            Assert.AreEqual(observation.DeviceUniqueId, roundTrip.DeviceUniqueId);
            Assert.AreEqual(observation.ObservedDeviceId, roundTrip.ObservedDeviceId);
            Assert.AreEqual(observation.TimeStamp, roundTrip.TimeStamp);
            Assert.AreEqual(observation.Rssi, roundTrip.Rssi);
            Assert.AreEqual(observation.DistanceEstimate, roundTrip.DistanceEstimate);
        }

        [TestMethod]
        public async Task BatchStoresOneRecordPerObservation()
        {
            var store = new Mock<IActivityRecordStore<DeviceProximityObservationActivityRecord>>();
            var captured = new List<DeviceProximityObservationActivityRecord>();
            store.Setup(s => s.InsertAsync(It.IsAny<DeviceProximityObservationActivityRecord>(), It.IsAny<CancellationToken>()))
                 .Callback<DeviceProximityObservationActivityRecord, CancellationToken>((record, _) => captured.Add(record))
                 .Returns(Task.CompletedTask);
            var repository = new DeviceProximityObservationRepo(store.Object);
            await repository.WriteAsync(new[] { Observation("Beacon01"), Observation("Beacon02") });
            Assert.AreEqual(2, captured.Count);
            Assert.AreEqual("Beacon01", captured[0].ObservedDeviceId);
            Assert.AreEqual("Beacon02", captured[1].ObservedDeviceId);
            Assert.AreNotEqual(captured[0].Id, captured[1].Id);
        }

        [TestMethod]
        public async Task QueryRetainsPaginationContinuation()
        {
            var store = new Mock<IActivityRecordStore<DeviceProximityObservationActivityRecord>>();
            HistoryQuery<DeviceProximityObservationActivityRecord> captured = null;
            store.Setup(s => s.QueryAsync(It.IsAny<HistoryQuery<DeviceProximityObservationActivityRecord>>(), It.IsAny<CancellationToken>()))
                 .Callback<HistoryQuery<DeviceProximityObservationActivityRecord>, CancellationToken>((query, _) => captured = query)
                 .ReturnsAsync(new StoragePageResult<DeviceProximityObservationActivityRecord>(
                     new[] { DeviceProximityObservationActivityRecord.From(Observation("Beacon01")) },
                     "next-token"));
            var repository = new DeviceProximityObservationRepo(store.Object);
            var response = await repository.QueryAsync("org-1", "repo-1", "device-1", new ListRequest
            {
                PageSize = 25,
                PageIndex = 2,
                NextPartitionKey = "repo-1/device-1",
                NextRowKey = "previous-token"
            });
            Assert.AreEqual("previous-token", captured.Page.ContinuationToken);
            Assert.AreEqual("repo-1/device-1", response.NextPartitionKey);
            Assert.AreEqual("next-token", response.NextRowKey);
            Assert.AreEqual("Beacon01", response.Model.Single().ObservedDeviceId);
        }
    }
}
