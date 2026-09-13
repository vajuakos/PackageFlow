using PackageFlow.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PackageFlow.Core.Models;

public class Package
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string TrackingNumber { get; set; }

    public PackageStatus Status { get; set; } = PackageStatus.Registered;
    public PackageSize Size { get; set; }
    public decimal WeightKg { get; set; }

    public DateOnly ScheduledPickupDate { get; set; }

    // Sender details
    public int? SenderUserId { get; set; }
    public AppUser? SenderUser { get; set; }

    // Details in case of Guest user
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPhone { get; set; }
    public Address SenderAddress { get; set; } = new();

    // Recipient
    public int? RecipientUserId { get; set; }
    public AppUser? RecipientUser { get; set; }

    public string RecipientName { get; set; }
    public string RecipientEmail { get; set; }
    public string RecipientPhone { get; set; }
    public Address DeliveryAddress { get; set; } = new();

    // Assigned couriers
    public int? PickupCourierId { get; set; }
    public AppUser? PickupCourier { get; set; }

    public int? DeliveryCourierId { get; set; }
    public AppUser? DeliveryCourier { get; set; }

    // Warehouse
    public int? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PickedUpAt { get; set; }
    public DateTime? ReceivedInWarehouseAt { get; set; }
    public DateTime? OutForDeliveryAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public ICollection<PackageStatusHistory> StatusHistory { get; set; } = new List<PackageStatusHistory>();
}