using System;
using System.Text;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace Dpx.Iot.Infrastructure.Devices
{
    public class CloudToDevice : ICloudToDevice, IDisposable
    {
        private readonly ServiceClient serviceClient;
        private bool isDisposed;

        internal CloudToDevice(string connectionString)
        {
            connectionString.ThrowIfParameterIsNullOrWhiteSpace(nameof(connectionString));
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SendMessageToDeviceAsync<T>(T data, Entities.Device device)
        {
            device.ThrowIfParameterIsNull(nameof(device));
            data.ThrowIfParameterIsNull(nameof(data));

            var jsonString = JsonConvert.SerializeObject(data);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);

            var message = new Message(messageBytes);
            message.Ack = DeliveryAcknowledgement.Full;

            await serviceClient.SendAsync(device.Id, message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            serviceClient?.Dispose();
            isDisposed = true;
        }
    }
}
