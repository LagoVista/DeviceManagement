using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Repos;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests;

[TestClass]
public sealed class FirmwareDownloadRequestRepoTests
{
    [TestMethod]
    public async Task AddRequestWritesOrganizationScopedApplicationDataRecord()
    {
        var store = new Mock<IApplicationDataStore>();
        FirmwareDownloadRequestRecord inserted = null;

        store.Setup(x => x.InsertAsync(
                It.IsAny<FirmwareDownloadRequestRecord>(),
                It.IsAny<CancellationToken>()))
            .Callback<FirmwareDownloadRequestRecord, CancellationToken>((record, _) => inserted = record)
            .Returns(Task.CompletedTask);

        var request = CreateRequest();
        var repository = new FirmwareDownloadRequestRepo(store.Object);

        await repository.AddRequestAsync(request);

        Assert.IsNotNull(inserted);
        Assert.AreEqual(request.DownloadId, inserted.Id.Value);
        Assert.AreEqual(request.OrgId, inserted.Organization.Id);
        Assert.AreEqual(request.Timestamp, inserted.CreationDate.Value);
        Assert.AreEqual(request.DeviceRepoId, inserted.DeviceRepoId);
        Assert.AreEqual(request.DeviceId, inserted.DeviceId);
        Assert.AreEqual(request.FirmwareRevisionId, inserted.FirmwareRevisionId);
    }

    [TestMethod]
    public async Task GetRequestUsesGloballyUniqueDownloadIdAndMapsRecord()
    {
        var request = CreateRequest();
        var record = FirmwareDownloadRequestRecord.FromRequest(request);
        var store = new Mock<IApplicationDataStore>();

        store.Setup(x => x.GetAsync<FirmwareDownloadRequestRecord>(
                It.Is<StorageKey>(key => key.Id == request.DownloadId && key.Scope == null),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var repository = new FirmwareDownloadRequestRepo(store.Object);
        var loaded = await repository.GetRequestAsync(request.DownloadId);

        Assert.IsNotNull(loaded);
        Assert.AreEqual(request.DownloadId, loaded.DownloadId);
        Assert.AreEqual(request.OrgId, loaded.OrgId);
        Assert.AreEqual(request.Status, loaded.Status);
        Assert.AreEqual(request.PercentRequested, loaded.PercentRequested);
        Assert.AreEqual(request.Timestamp, loaded.Timestamp);
    }

    [TestMethod]
    public async Task UpdateRequestUsesOrganizationScopedApplicationDataIdentity()
    {
        var store = new Mock<IApplicationDataStore>();
        FirmwareDownloadRequestRecord updated = null;
        var request = CreateRequest();
        request.Status = "Downloading";
        request.PercentRequested = 40;

        store.Setup(x => x.UpdateAsync(
                It.IsAny<FirmwareDownloadRequestRecord>(),
                It.IsAny<CancellationToken>()))
            .Callback<FirmwareDownloadRequestRecord, CancellationToken>((record, _) => updated = record)
            .Returns(Task.CompletedTask);

        var repository = new FirmwareDownloadRequestRepo(store.Object);
        await repository.UpdateRequestAsync(request);

        Assert.IsNotNull(updated);
        Assert.AreEqual(request.DownloadId, updated.Id.Value);
        Assert.AreEqual(request.OrgId, updated.Organization.Id);
        Assert.AreEqual("Downloading", updated.Status);
        Assert.AreEqual(40, updated.PercentRequested);
    }

    [TestMethod]
    public async Task DeviceHistoryIsOrganizationScopedSortedAndFullyPaged()
    {
        var orgId = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        var first = CreateRequest(orgId);
        first.Timestamp = "2026-11-26T12:00:00.000Z";
        var second = CreateRequest(orgId);
        second.Timestamp = "2026-11-26T11:00:00.000Z";

        var pages = new Queue<StoragePageResult<FirmwareDownloadRequestRecord>>();
        pages.Enqueue(new StoragePageResult<FirmwareDownloadRequestRecord>(
            new[] { FirmwareDownloadRequestRecord.FromRequest(first) }, "next-page"));
        pages.Enqueue(new StoragePageResult<FirmwareDownloadRequestRecord>(
            new[] { FirmwareDownloadRequestRecord.FromRequest(second) }));

        var observedQueries = new List<StorageQuery<FirmwareDownloadRequestRecord>>();
        var store = new Mock<IApplicationDataStore>();
        store.Setup(x => x.QueryAsync(
                It.IsAny<StorageQuery<FirmwareDownloadRequestRecord>>(),
                It.IsAny<CancellationToken>()))
            .Callback<StorageQuery<FirmwareDownloadRequestRecord>, CancellationToken>((query, _) => observedQueries.Add(query))
            .Returns(() => Task.FromResult(pages.Dequeue()));

        var repository = new FirmwareDownloadRequestRepo(store.Object);
        var result = (await repository.GetForDeviceAsync(orgId, first.DeviceRepoId, first.DeviceId)).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(first.DownloadId, result[0].DownloadId);
        Assert.AreEqual(second.DownloadId, result[1].DownloadId);
        Assert.AreEqual(2, observedQueries.Count);

        var filters = observedQueries[0].Filters.ToDictionary(filter => filter.Field, filter => filter.Value);
        Assert.AreEqual(orgId, filters["Organization.Id"]);
        Assert.AreEqual(first.DeviceRepoId, filters["DeviceRepoId"]);
        Assert.AreEqual(first.DeviceId, filters["DeviceId"]);
        Assert.AreEqual("Timestamp", observedQueries[0].Sorts.Single().Field);
        Assert.AreEqual(StorageSortDirection.Descending, observedQueries[0].Sorts.Single().Direction);
        Assert.AreEqual(1000, observedQueries[0].Page.PageSize);
        Assert.IsNull(observedQueries[0].Page.ContinuationToken);
        Assert.AreEqual("next-page", observedQueries[1].Page.ContinuationToken);
    }

    [TestMethod]
    public void StartupRegistersFirmwareRequestIndexesForApplicationData()
    {
        var services = new ServiceCollection();
        LagoVista.IoT.DeviceManagement.Repos.Startup.ConfigureServices(services);

        var options = services
            .Single(descriptor => descriptor.ServiceType == typeof(ApplicationDataStoreOptions<FirmwareDownloadRequestRecord>))
            .ImplementationInstance as ApplicationDataStoreOptions<FirmwareDownloadRequestRecord>;

        Assert.IsNotNull(options);
        CollectionAssert.AreEquivalent(
            new[] { "DeviceRepoId", "DeviceId", "Timestamp" },
            options.Definition.IndexedFields.ToArray());
    }

    private static FirmwareDownloadRequest CreateRequest(string orgId = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
    {
        var now = UtcTimestamp.Now;
        return new FirmwareDownloadRequest
        {
            DownloadId = Guid.NewGuid().ToString("N").ToUpperInvariant(),
            OrgId = orgId,
            Expired = false,
            Status = "Requested",
            PercentRequested = 0,
            DeviceRepoId = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
            DeviceId = "device-123",
            FirmwareName = "controller",
            Timestamp = now.Value,
            ExpiresUTC = now.AddDays(1).Value,
            FirmwareId = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC",
            FirmwareRevisionId = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD",
            Error = "noerror"
        };
    }
}
