namespace PackageFlow.Shared.DTOs.Admin
{
    public class AdminStatisticsResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalPackageCount { get; set; }
        public int ActivePackageCount { get; set; }
        public int DeliveredInPeriodCount { get; set; }

        public double? AverageDeliveryTimeHours { get; set; }

        public double? AverageTotalTransitTimeHours { get; set; }

        public List<WarehouseStatisticsResult> Warehouses { get; set; } = new();
        public List<CourierStatisticsResult> Couriers { get; set; } = new();
        public List<DailyPackageCountResult> DailyCounts { get; set; } = new();
    }
}
