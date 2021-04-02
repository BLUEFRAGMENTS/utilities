using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Dpx.Iot.Infrastructure.Extensions;
using Microsoft.Azure.Devices;

namespace Dpx.Iot.Infrastructure.Devices
{
    internal class RegistryManagerWrapper : IRegistryManagerWrapper
    {
        private RegistryManager registryManager;

        public RegistryManagerWrapper(string connectionString)
        {
            connectionString.ThrowIfParameterIsNullOrWhiteSpace(nameof(connectionString));

            HostName = connectionString.ExtractHostName();

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public string HostName { get; private set; }

        public async Task<Device?> AddDeviceAsync(Device device)
        {
            try
            {
                return await registryManager.AddDeviceAsync(device);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            registryManager?.Dispose();
        }

        public async Task<Device?> GetDeviceAsync(string deviceId)
        {
            try
            {
                return await registryManager.GetDeviceAsync(deviceId);
            } 
            catch (Exception) 
            {
                return null;
            }
        }

        public Task RemoveDeviceAsync(Device device)
        {
            try
            {
                return registryManager.RemoveDeviceAsync(device);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }
    }
}
