using System;
using System.Threading.Tasks;
using Dpx.Iot.Infrastructure.Entities;

namespace Dpx.Iot.Infrastructure.Devices
{
    public interface ICloudToDevice : IDisposable
    {
        Task SendMessageToDeviceAsync<T>(T data, Device device);
    }
}