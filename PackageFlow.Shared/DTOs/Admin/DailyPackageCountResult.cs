namespace PackageFlow.Shared.DTOs.Admin
{
    public class DailyPackageCountResult
    {
        public DateOnly Date { get; set; }
        public int CreatedCount { get; set; }
        public int PickedUpCount { get; set; }
        public int DeliveredCount { get; set; }
    }
}
