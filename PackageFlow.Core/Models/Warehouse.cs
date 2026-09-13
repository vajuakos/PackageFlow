namespace PackageFlow.Core.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public int MaxCapacity { get; set; }

        public ICollection<Package> StoredPackages { get; set; } = new List<Package>();
        public ICollection<WarehouseCapacityLog> CapacityLogs { get; set; } = new List<WarehouseCapacityLog>();
    }
}
