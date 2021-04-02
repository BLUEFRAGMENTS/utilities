using System;
using System.Threading.Tasks;
using Bluefragments.Utilities.Iot.IotHub.Entities;

namespace Bluefragments.Utilities.Iot.IotHub.Devices
{
    public interface ICloudToDevice : IDisposable
    {
        Task SendMessageToDeviceAsync<T>(T data, Device device);
    }
}