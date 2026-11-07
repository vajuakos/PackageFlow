namespace PackageFlow.Shared.DTOs.Admin
{
    public class WarehouseUtilizationPointResult
    {
        public DateOnly Date { get; set; }
        public int PackageCount { get; set; }
        public double UtilizationPercent { get; set; }
    }
}
