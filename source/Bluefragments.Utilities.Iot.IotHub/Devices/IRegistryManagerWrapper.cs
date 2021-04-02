using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Dpx.Iot.Infrastructure.Devices
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