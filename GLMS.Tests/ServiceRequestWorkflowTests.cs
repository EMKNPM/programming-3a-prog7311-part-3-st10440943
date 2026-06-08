using System;
using System.Threading.Tasks;
using GLMS.Web.Data;
using GLMS.Web.Models;
using GLMS.Web.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Tests
{
    public class ServiceRequestWorkflowTests
    {
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateRequest_ExpiredContract_Fails()
        {
            var db = GetInMemoryDb();
            db.Contracts.Add(new Contract
            {
                Id = 1,
                Status = ContractStatus.Expired,
                ServiceLevel = "Basic",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-1),
                ClientId = 1
            });
            await db.SaveChangesAsync();

            var service = new ServiceRequestService(db);
            var (success, error) = await service.CreateAsync(new ServiceRequest
            {
                ContractId = 1,
                Description = "Test",
                CostUSD = 100
            });

            Assert.False(success);
            Assert.Contains("Expired", error);
        }

        [Fact]
        public async Task CreateRequest_OnHoldContract_Fails()
        {
            var db = GetInMemoryDb();
            db.Contracts.Add(new Contract
            {
                Id = 1,
                Status = ContractStatus.OnHold,
                ServiceLevel = "Basic",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ClientId = 1
            });
            await db.SaveChangesAsync();

            var service = new ServiceRequestService(db);
            var (success, error) = await service.CreateAsync(new ServiceRequest
            {
                ContractId = 1,
                Description = "Test",
                CostUSD = 100
            });

            Assert.False(success);
            Assert.Contains("On Hold", error);
        }

        [Fact]
        public async Task CreateRequest_ActiveContract_Succeeds()
        {
            var db = GetInMemoryDb();
            db.Contracts.Add(new Contract
            {
                Id = 1,
                Status = ContractStatus.Active,
                ServiceLevel = "Premium",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ClientId = 1
            });
            await db.SaveChangesAsync();

            var service = new ServiceRequestService(db);
            var (success, error) = await service.CreateAsync(new ServiceRequest
            {
                ContractId = 1,
                Description = "Shipment to EU",
                CostUSD = 500
            });

            Assert.True(success);
            Assert.Empty(error);
        }
    }
}