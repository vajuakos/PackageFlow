namespace PackageFlow.Core.Models
{
    public class WarehouseCapacityLog
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public int CurrentPackageCount { get; set; }
        public int MaxCapacitySnapshot { get; set; }
    }
}
