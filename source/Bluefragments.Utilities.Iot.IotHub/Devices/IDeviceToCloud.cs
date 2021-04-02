using Dpx.Iot.Infrastructure.Devices;
using Dpx.Iot.Infrastructure.Entities;

namespace Bluefragments.Iot.Devices
{
    public interface IDeviceToCloud
    {
        /// <summary>
        /// Creates a simulated device, which simulates real device features, like sending data points.
        /// </summary>
        SimulatedDevice CreateSimulatedDevice(Device device);
    }
}