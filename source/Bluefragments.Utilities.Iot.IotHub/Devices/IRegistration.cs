using System;
using System.Threading.Tasks;
using Dpx.Iot.Infrastructure.Entities;

namespace Dpx.Iot.Infrastructure.Devices
{
    public interface IRegistration : IDisposable
    {
        Task<Device?> GetDeviceAsync(string deviceId);

        Task<Device?> RegisterDeviceAsync(string deviceId);

        Task<bool> RemoveDeviceAsync(Device iotDevice);

        Task<bool> RemoveDeviceAsync(string id);
    }
}