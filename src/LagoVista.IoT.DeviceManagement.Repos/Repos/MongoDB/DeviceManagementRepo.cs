using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core;
using System.Linq;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos.MongoDB
{
    public class DeviceManagementRepo : IDeviceManagementRepo
    {

        MongoClient _client;
        IMongoCollection<Device> _deviceCollection;

        public DeviceManagementRepo()
        {
            _client = new MongoClient("mongodb://127.0.0.1:27017");
            var db = _client.GetDatabase("nuviot");
            _deviceCollection = db.GetCollection<Device>("Devices");
        }

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository repo, Device device)
        {
            device.DeviceRepository = new LagoVista.Core.Models.EntityHeader() { Id = repo.Id, Text = repo.Name };
            await _deviceCollection.InsertOneAsync(device);
            return InvokeResult.Success;
        }

        public async Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string deviceId, string orgid)
        {
            return (await _deviceCollection.FindAsync(dev => dev.Id == deviceId && dev.DeviceRepository.Id == repo.Id)).Any();
        }

        public Task DeleteDeviceAsync(DeviceRepository repo, string id)
        {
            return _deviceCollection.FindOneAndDeleteAsync(dev => dev.Id == id);
        }

        public Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId)
        {
            return _deviceCollection.FindOneAndDeleteAsync(dev => dev.DeviceId == deviceId && dev.DeviceRepository.Id == repo.Id);
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id)
        {
            return (await _deviceCollection.FindAsync(dev => dev.DeviceId == id)).FirstOrDefault();
        }

        public async Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id)
        {
            return (await _deviceCollection.FindAsync(dev => dev.Id == id)).FirstOrDefault();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository repo, string orgId, int top, int take)
        {
            var devices = await (await _deviceCollection.FindAsync(dev => dev.DeviceRepository.Id == repo.Id)).ToListAsync();
            return from devs in devices select devs.CreateSummary();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(DeviceRepository repo, Device device)
        {
            return _deviceCollection.ReplaceOneAsync(dev => dev.Id == device.Id, device);
        }
    }
}
