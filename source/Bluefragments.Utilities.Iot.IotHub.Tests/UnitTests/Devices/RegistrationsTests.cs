using System;
using System.Threading.Tasks;
using Dpx.Iot.Infrastructure.Devices;
using Dpx.Iot.Infrastructure.Entities;
using FakeItEasy;
using Microsoft.Azure.Devices;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Devices
{
    public class RegistrationsTests : IDisposable
    {
        private const string ValidDeviceId = "1";
        private const string InvalidDeviceId = "2";

        private readonly Registration registration;
        private readonly IRegistryManagerWrapper regManagerWrapper;

        private bool isDisposed;

        public RegistrationsTests()
        {
            regManagerWrapper = A.Fake<IRegistryManagerWrapper>();

            Microsoft.Azure.Devices.Device dev = new Microsoft.Azure.Devices.Device(ValidDeviceId);
            dev.Authentication = new AuthenticationMechanism();

            A.CallTo(() => regManagerWrapper.HostName).Returns(string.Empty);

            A.CallTo(() => regManagerWrapper.GetDeviceAsync(ValidDeviceId)).Returns(Task.FromResult<Microsoft.Azure.Devices.Device?>(dev));
            A.CallTo(() => regManagerWrapper.GetDeviceAsync(InvalidDeviceId)).Returns(Task.FromResult<Microsoft.Azure.Devices.Device?>(null));

            A.CallTo(() => regManagerWrapper.AddDeviceAsync(null!)).Returns(Task.FromResult<Microsoft.Azure.Devices.Device?>(null));

            registration = new Registration(regManagerWrapper);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async void GetDeviceAsync_NullDevice_ReturnsNull()
        {
            var dev = await registration.GetDeviceAsync(InvalidDeviceId);
            Assert.Null(dev);
        }

        [Fact]
        public async void GetDeviceAsync_ValidDevice_ReturnsDevice()
        {
            var dev = await registration.GetDeviceAsync(ValidDeviceId);
            Assert.NotNull(dev);
        }

        [Fact]
        public async void GetDeviceAsync_EmptyDeviceId_ThrowsArgumentOutOfRangeException()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await registration.GetDeviceAsync(string.Empty));
        }

        [Fact]
        public async void RegisterDeviceAsync_NullDevice_ReturnsNull()
        {
            A.CallTo(() => regManagerWrapper.AddDeviceAsync(A<Microsoft.Azure.Devices.Device>.Ignored)).Returns(Task.FromResult<Microsoft.Azure.Devices.Device?>(null));
            var dev = await registration.RegisterDeviceAsync(InvalidDeviceId);
            Assert.Null(dev);
        }

        [Fact]
        public async void RegisterDeviceAsync_ValidDevice_ReturnsDevice()
        {
            Microsoft.Azure.Devices.Device dev = new Microsoft.Azure.Devices.Device(ValidDeviceId);
            dev.Authentication = new AuthenticationMechanism();

            A.CallTo(() => regManagerWrapper.AddDeviceAsync(A<Microsoft.Azure.Devices.Device>.Ignored)).Returns(Task.FromResult<Microsoft.Azure.Devices.Device?>(dev));
            var result = await registration.RegisterDeviceAsync(ValidDeviceId);
            Assert.NotNull(result);
        }

        [Fact]
        public async void RegisterDeviceAsync_EmptyDeviceId_ThrowsArgumentOutOfRangeException()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await registration.GetDeviceAsync(string.Empty));
        }

        [Fact]
        public async void RemoveDeviceAsync_EmptyDeviceId_ThrowsArgumentOutOfRangeException()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await registration.RemoveDeviceAsync(string.Empty));
        }

        [Fact]
        public async void RemoveDeviceAsync_ValidDeviceId_ThrowsNoException()
        {
            await registration.RemoveDeviceAsync(ValidDeviceId);
            Assert.True(true);
        }

        [Fact]
        public void Ctor_NullRegistryManagerWrapper_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Registration(null!));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            registration?.Dispose();
            isDisposed = true;
        }
    }
}
