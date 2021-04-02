using System;
using Bluefragments.Utilities.Extensions;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Bluefragments.Utilities.Iot.IotHub.Entities;
using Bluefragments.Utilities.Iot.IotHub.Extensions;
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
