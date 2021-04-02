using System;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Bluefragments.Utilities.Iot.IotHub.Entities;
using Bluefragments.Utilities.Iot.IotHub.Extensions;

namespace Bluefragments.Utilities.Iot.IotHub.Devices
{
    public class Registration : IRegistration, IDisposable
    {
        private readonly IRegistryManagerWrapper registryManagerWrapper;
        private bool isDisposed;

        internal Registration(IRegistryManagerWrapper wrapper)
        {
            wrapper.ThrowIfParameterIsNull(nameof(wrapper));
            registryManagerWrapper = wrapper;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<Device?> GetDeviceAsync(string deviceId)
        {
            deviceId.ThrowIfParameterIsNullOrWhiteSpace(nameof(deviceId));

            var device = await registryManagerWrapper.GetDeviceAsync(deviceId);
            if (device == null)
            {
                return null;
            }

            return new Device(device.Id, device.Authentication.SymmetricKey.PrimaryKey, registryManagerWrapper.HostName);
        }

        public async Task<Device?> RegisterDeviceAsync(string deviceId)
        {
            deviceId.ThrowIfParameterIsNullOrWhiteSpace(nameof(deviceId));

            var device = new Microsoft.Azure.Devices.Device(deviceId);
            device = await registryManagerWrapper.AddDeviceAsync(device);

            if (device == null)
            {
                return null;
            }

            return new Device(device.Id, device.Authentication.SymmetricKey.PrimaryKey, registryManagerWrapper.HostName);
        }

        public async Task<bool> RemoveDeviceAsync(Device device)
        {
            device.ThrowIfParameterIsNull(nameof(device));
            return await RemoveDeviceAsync(device.Id);
        }

        public async Task<bool> RemoveDeviceAsync(string id)
        {
            id.ThrowIfParameterIsNullOrWhiteSpace(nameof(id));

            var iotDevice = await registryManagerWrapper.GetDeviceAsync(id);
            if (iotDevice == null)
            {
                return false;
            }

            await registryManagerWrapper.RemoveDeviceAsync(iotDevice);
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            registryManagerWrapper?.Dispose();
            isDisposed = true;
        }
    }
}
