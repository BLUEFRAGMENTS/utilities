using System;
using System.Threading.Tasks;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Bluefragments.Utilities.Iot.IotHub.Entities;
using Xunit;

namespace Bluefragments.Utilities.Iot.IotHub.Tests.IntegrationTests.Devices
{
    [CollectionDefinition(nameof(RegistrationsTests), DisableParallelization = true)]
    public class RegistrationsTests : TestBase, IAsyncLifetime
    {
        private const string DeviceId = "TestDevice";
        private readonly Registration deviceRegistration;

        public RegistrationsTests()
        {
            var connectionString = Configuration["IotHubConnectionString"];
            deviceRegistration = new Registration(new RegistryManagerWrapper(connectionString));
        }

        public async Task InitializeAsync()
        {
            await deviceRegistration.RemoveDeviceAsync(DeviceId);
        }

        public async Task DisposeAsync()
        {
            await deviceRegistration.RemoveDeviceAsync(DeviceId);
            deviceRegistration.Dispose();
        }

        [Fact]
        public async Task AddDeviceAsync_AddsDevice()
        {
            var device = await deviceRegistration.RegisterDeviceAsync(DeviceId);

            Assert.NotNull(device);

            Assert.Equal(DeviceId, device!.Id);
        }

        [Fact]
        public async Task GetDeviceAsync_DeviceExists_ReturnsDevice()
        {
            var device = await deviceRegistration.RegisterDeviceAsync(DeviceId);
            Assert.NotNull(device);

            var result = await deviceRegistration.GetDeviceAsync(device!.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDeviceAsync_DeviceDoesNotExist_ReturnsNull()
        {
            var result = await deviceRegistration.GetDeviceAsync(Guid.NewGuid().ToString());
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteDeviceAsync_DevicesExists_DeletesDevice()
        {
            var device = await deviceRegistration.RegisterDeviceAsync(DeviceId);
            Assert.NotNull(device);

            var didRemove = await deviceRegistration.RemoveDeviceAsync(device!);
            Assert.True(didRemove);
        }

        [Fact]
        public async Task DeleteDeviceAsync_DeviceDoesNotExist_DoesNotDeleteDevice()
        {
            var device = new Device(Guid.NewGuid().ToString(), string.Empty, string.Empty);
            var didRemove = await deviceRegistration.RemoveDeviceAsync(device);

            Assert.False(didRemove);
        }
    }
}
