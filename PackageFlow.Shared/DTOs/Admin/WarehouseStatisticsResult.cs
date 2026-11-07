namespace PackageFlow.Shared.DTOs.Admin
{
    public class WarehouseStatisticsResult
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }

        public int CurrentPackageCount { get; set; }
        public double CurrentUtilizationPercent { get; set; }

        public int PeakPackageCount { get; set; }
        public double AverageUtilizationPercent { get; set; }

        public List<WarehouseUtilizationPointResult> History { get; set; } = new();
    }
}
