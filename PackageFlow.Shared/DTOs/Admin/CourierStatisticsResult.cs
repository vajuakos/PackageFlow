namespace PackageFlow.Shared.DTOs.Admin
{
    public class CourierStatisticsResult
    {
        public int CourierUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string VehiclePlateNumber { get; set; } = string.Empty;

        public int PickupCount { get; set; }
        public int DeliveredCount { get; set; }

        public double AveragePackagesPerDay { get; set; }

        public double? AverageDeliveryTimeHours { get; set; }

        public int ActiveAssignmentCount { get; set; }
    }
}
