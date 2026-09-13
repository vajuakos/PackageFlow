namespace PackageFlow.Core.Enums
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
        Cancelled = 8
    }
}
