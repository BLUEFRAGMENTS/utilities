using System;
using System.Threading.Tasks;
using Bluefragments.Utilities.Iot.IotHub.Entities;

namespace Bluefragments.Utilities.Iot.IotHub.Devices
{
    public interface IRegistration : IDisposable
    {
        Task<Device?> GetDeviceAsync(string deviceId);

        Task<Device?> RegisterDeviceAsync(string deviceId);

        Task<bool> RemoveDeviceAsync(Device iotDevice);

        Task<bool> RemoveDeviceAsync(string id);
    }
}