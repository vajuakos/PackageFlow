namespace PackageFlow.Shared.Enums
{
    public enum PackageStatus
    {
        Registered = 0,
        PickupAssigned = 1,
        PickedUp = 2,
        InWarehouse = 3,
        DeliveryAssigned = 4,
        OutForDelivery = 5,
        Delivered = 6,
        Failed = 7,
        Cancelled = 8,
        Modified = 9
    }

    public static class PackageStatusExtensions
    {
        public static string ToDisplayName(this PackageStatus status) => status switch
        {
            PackageStatus.Registered => "Előjegyezve",
            PackageStatus.PickupAssigned => "Felvétel kiosztva",
            PackageStatus.PickedUp => "Felvéve",
            PackageStatus.InWarehouse => "Raktárban",
            PackageStatus.DeliveryAssigned => "Kiszállítás kiosztva",
            PackageStatus.OutForDelivery => "Kiszállítás alatt",
            PackageStatus.Delivered => "Kézbesítve",
            PackageStatus.Failed => "Sikertelen kézbesítés",
            PackageStatus.Cancelled => "Törölve",
            PackageStatus.Modified => "Módosítva",
            _ => status.ToString()
        };
    }
}
