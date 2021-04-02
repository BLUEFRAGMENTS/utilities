using System;
using Bluefragments.Utilities.Extensions;
using Dpx.Iot.Infrastructure.Devices;
using Dpx.Iot.Infrastructure.Entities;
using Dpx.Iot.Infrastructure.Extensions;
using Microsoft.Azure.Devices.Client;

namespace Bluefragments.Iot.Devices
{
    public class DeviceToCloud : IDeviceToCloud
    {
        public SimulatedDevice CreateSimulatedDevice(Device device)
        {
            device.ThrowIfParameterIsNull(nameof(device));

            var deviceConnectionString = device.GetDeviceConnectionString();
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);

            return new SimulatedDevice(deviceClient);
        }
    }
}
