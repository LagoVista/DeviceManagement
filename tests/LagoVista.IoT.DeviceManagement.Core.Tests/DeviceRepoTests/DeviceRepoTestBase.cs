using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceRepoTests
{
    public class DeviceRepoTestBase : ValidationBase
    {
        protected DeviceRepository GetValidRepo()
        {
            EntityHeader _user = EntityHeader.Create("3367B1522AF441F39238A85A80B94D33", "User");
            EntityHeader _org = EntityHeader.Create("5567B1522AF441F39238A85A80B94D33", "User");

            var repo = new DeviceRepository()
            {
                Id = "6C67B1522AF441F39238A85A80B94D39",
                CreatedBy = _user,
                CreationDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedBy = _user,
                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                OwnerOrganization = _org,
                Name = "My Good Repo",
                Key = "mygoodrepo",
                Subscription = EntityHeader.Create("id123", "idtext"),
                DeviceCapacity = EntityHeader.Create("dev123", "capac"),
                StorageCapacity = EntityHeader.Create("storage", "storage"),
                RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT)
            };

            return repo;
        }
    }
}
