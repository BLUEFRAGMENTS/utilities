using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Bluefragments.Utilities.Iot.IotHub.Devices
{
    public interface IRegistryManagerWrapper
    {
        string HostName { get; }

        void Dispose();

        Task<Device?> GetDeviceAsync(string deviceId);

        Task<Device?> AddDeviceAsync(Device device);

        Task RemoveDeviceAsync(Device iotDevice);
    }
}