namespace Dpx.Iot.Infrastructure.Entities
{
    public class Device
    {
        public Device()
        {
            Id = string.Empty;
            PrimaryKey = string.Empty;
            HostName = string.Empty;
        }

        public Device(string id, string primaryKey, string hostName)
        {
            Id = id;
            PrimaryKey = primaryKey;
            HostName = hostName;
        }

        public string Id { get; set; }

        public string PrimaryKey { get; set; }

        public string HostName { get; set; }
    }
}
