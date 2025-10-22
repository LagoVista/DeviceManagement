// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: be3598b05688bc20f37277eb2ddb8a15b68b0f76bb516041f9cd1f53e6ccb1a5
// IndexVersion: 0
// --- END CODE INDEX META ---
// If you want to test auto create of the container, uncomment this line, however once it removes the old one, you can't create a new one for
// an underterminant amount of time.
//#define SHOULD_TEST_CREATE_BLOB_CONTAINER


using Azure.Storage.Blobs;
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Tests;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.MediaIntegrationTests.MediaTests
{
    [TestClass]
    public class MediaFileRepoTests : ValidationBase
    {
        /* 
         * NOte if this fails run again, it will delete the test contianer after initial run, and sometimes we get a delete conflict, should never happen in production,
         * this is really an integration test, not sure how much time should be put in to fixing. */


        private static DeviceRepository GetTestDeviceRepo()
        {
            var deviceRepo = new DeviceRepository()
            {
                DeviceArchiveStorageSettings = CloudStorage.Utils.TestConnections.DevTableStorageDB,
                Key = "testrepo",
                Id = "890C3F4F480C4FF283F7C9B16CB5F368"
            };

            return deviceRepo;
        }

        [TestMethod]
        public async Task Media_Blob_DownloadTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);

            Console.WriteLine("Media Item Added.");

            var downloadResult = await mediaRepo.GetMediaAsync(GetTestDeviceRepo(), fileName);
            AssertSuccessful(downloadResult.ToInvokeResult());

            Assert.AreEqual(imageBytes.Length, downloadResult.Result.Length);

            for (var idx = 0; idx < imageBytes.Length; ++idx)
            {
                Assert.AreEqual(imageBytes[idx], downloadResult.Result[idx], $"Difference in byte array at:{idx}");
            }
        }

        [TestMethod]
        public async Task Media_Blob_DeleteBlobTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);

            Console.WriteLine("Media Item Added.");

            var deleteResult = await mediaRepo.DeleteMediaAsync(GetTestDeviceRepo(), fileName);
            AssertSuccessful(deleteResult.ToInvokeResult());

            var getResult = await mediaRepo.GetMediaAsync(GetTestDeviceRepo(), fileName);
            AssertInvalidError(getResult.ToInvokeResult(), "The remote server returned an error: (404) Not Found.", "Unhandled Excpetion");
        }

        [TestMethod]
        public async Task Media_Blob_UploadTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);
        }

#if SHOULD_TEST_CREATE_BLOB_CONTAINER

        private static async Task RemoveTestContainerAsync()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));

            var accountId = CloudStorage.Utils.TestConnections.DevTableStorageDB.AccountId;
            var accessKey = CloudStorage.Utils.TestConnections.DevTableStorageDB.AccessKey;
            var containerName = GetTestDeviceRepo().GetDeviceMediaStorageName();

            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountId};AccountKey={accessKey}";
            var blobClient = new BlobServiceClient(connectionString);
            try
            {
                var blobContainerClient = blobClient.GetBlobContainerClient(containerName);
                await blobContainerClient.DeleteAsync();
            }
            catch (Exception)
            { 
                /* NOP */
            }

        }

        [ClassCleanup]
        public static async Task TestCleanup()
        {
                   await RemoveTestContainerAsync();
        }
#endif
    }
}
