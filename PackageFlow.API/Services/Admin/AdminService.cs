using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.DTOs.Admin;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.WarehouseManager;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Services.Admin
{
    public class AdminService : IAdminService
    {
        private static readonly PackageStatus[] InWarehouseStatuses =
        [
            PackageStatus.InWarehouse,
            PackageStatus.DeliveryAssigned
        ];

        private static readonly PackageStatus[] PickupReassignableStatuses =
        [
            PackageStatus.Registered,
            PackageStatus.PickupAssigned,
            PackageStatus.PickedUp
        ];

        private static readonly PackageStatus[] DeliveryReassignableStatuses =
        [
            PackageStatus.InWarehouse,
            PackageStatus.DeliveryAssigned,
            PackageStatus.OutForDelivery,
            PackageStatus.Failed
        ];

        private static readonly string[] StaffRoles =
        [
            UserRoles.Courier,
            UserRoles.WarehouseManager,
            UserRoles.Admin
        ];

        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(AppDbContext dbContext, UserManager<AppUser> userManager, ILogger<AdminService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger;
        }

        public async Task<List<AdminPackageResult>> GetPackagesAsync()
        {
            return await _dbContext.Packages
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new AdminPackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,

                    SenderUserId = p.SenderUserId,
                    SenderName = p.SenderUser != null ? p.SenderUser.FullName : p.SenderName,
                    SenderEmail = p.SenderEmail,
                    SenderPhone = p.SenderPhone,
                    SenderAddress = new AddressResult
                    {
                        Country = p.SenderAddress.Country,
                        State = p.SenderAddress.State,
                        PostalCode = p.SenderAddress.PostalCode,
                        City = p.SenderAddress.City,
                        Street = p.SenderAddress.Street,
                        StreetNumber = p.SenderAddress.StreetNumber,
                        BuildingDetails = p.SenderAddress.BuildingDetails
                    },

                    RecipientUserId = p.RecipientUserId,
                    RecipientName = p.RecipientName,
                    RecipientEmail = p.RecipientEmail,
                    RecipientPhone = p.RecipientPhone,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },

                    PickupCourierId = p.PickupCourierId,
                    PickupCourierName = p.PickupCourier != null ? p.PickupCourier.FullName : null,
                    DeliveryCourierId = p.DeliveryCourierId,
                    DeliveryCourierName = p.DeliveryCourier != null ? p.DeliveryCourier.FullName : null,

                    WarehouseName = p.Warehouse != null ? p.Warehouse.Name : null,

                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .ToListAsync();
        }

        public async Task<AdminPackageResult?> GetPackageAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.", nameof(trackingNumber));

            var cleanTrackingNumber = trackingNumber.Trim();

            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.TrackingNumber == cleanTrackingNumber)
                .Select(p => new AdminPackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,

                    SenderUserId = p.SenderUserId,
                    SenderName = p.SenderUser != null ? p.SenderUser.FullName : p.SenderName,
                    SenderEmail = p.SenderEmail,
                    SenderPhone = p.SenderPhone,
                    SenderAddress = new AddressResult
                    {
                        Country = p.SenderAddress.Country,
                        State = p.SenderAddress.State,
                        PostalCode = p.SenderAddress.PostalCode,
                        City = p.SenderAddress.City,
                        Street = p.SenderAddress.Street,
                        StreetNumber = p.SenderAddress.StreetNumber,
                        BuildingDetails = p.SenderAddress.BuildingDetails
                    },

                    RecipientUserId = p.RecipientUserId,
                    RecipientName = p.RecipientName,
                    RecipientEmail = p.RecipientEmail,
                    RecipientPhone = p.RecipientPhone,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },

                    PickupCourierId = p.PickupCourierId,
                    PickupCourierName = p.PickupCourier != null ? p.PickupCourier.FullName : null,
                    DeliveryCourierId = p.DeliveryCourierId,
                    DeliveryCourierName = p.DeliveryCourier != null ? p.DeliveryCourier.FullName : null,

                    WarehouseName = p.Warehouse != null ? p.Warehouse.Name : null,

                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePackageAsync(string trackingNumber, AdminUpdatePackageRequest request, int adminId)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.", nameof(trackingNumber));

            if (adminId <= 0)
                throw new ArgumentOutOfRangeException(nameof(adminId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
                return false;

            if (request.Status == PackageStatus.PickupAssigned && package.PickupCourierId == null)
                return false;

            if ((request.Status == PackageStatus.DeliveryAssigned || request.Status == PackageStatus.OutForDelivery)
                && package.DeliveryCourierId == null)
                return false;

            var now = DateTime.UtcNow;
            var statusChanged = package.Status != request.Status;

            if (statusChanged)
                await ApplyStatusTimestampsAsync(package, request.Status, now);

            package.Status = request.Status;
            package.Size = request.Size;
            package.WeightKg = request.WeightKg;
            package.ScheduledPickupDate = DateOnly.FromDateTime(request.ScheduledPickupDate);

            if (!string.Equals(package.RecipientEmail, request.RecipientEmail, StringComparison.OrdinalIgnoreCase))
            {
                package.RecipientUserId = await _dbContext.Users
                    .Where(u => u.Email == request.RecipientEmail.ToLower())
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync();
            }

            package.RecipientName = request.RecipientName;
            package.RecipientEmail = request.RecipientEmail;
            package.RecipientPhone = request.RecipientPhone;
            package.DeliveryAddress = new Address
            {
                Country = request.DeliveryCountry,
                State = request.DeliveryState,
                PostalCode = request.DeliveryPostalCode,
                City = request.DeliveryCity,
                Street = request.DeliveryStreet,
                StreetNumber = request.DeliveryStreetNumber,
                BuildingDetails = request.DeliveryBuildingDetails
            };

            _dbContext.PackageStatusHistories.Add(new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = statusChanged ? request.Status : PackageStatus.Modified,
                Timestamp = now,
                ChangedByUserId = adminId,
                Note = statusChanged
                    ? "The administrator modified the package details and status."
                    : "The administrator modified the package details."
            });

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Package updated by admin. TrackingNumber: {TrackingNumber}, AdminId: {AdminId}, Status: {Status}",
                trackingNumber, adminId, request.Status);

            return true;
        }

        public async Task<bool> DeletePackageAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.", nameof(trackingNumber));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
                return false;

            _dbContext.Packages.Remove(package);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Package deleted by admin. TrackingNumber: {TrackingNumber}", trackingNumber);

            return true;
        }

        public async Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId, int adminId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.", nameof(trackingNumber));

            if (courierUserId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierUserId));

            if (adminId <= 0)
                throw new ArgumentOutOfRangeException(nameof(adminId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
                return false;

            // Unlike the warehouse manager, the admin can reassign after the pickup too.
            if (!PickupReassignableStatuses.Contains(package.Status))
                return false;

            var courierExists = await _dbContext.CourierProfiles
                .AnyAsync(cp => cp.UserId == courierUserId);

            if (!courierExists)
                return false;

            bool isReassignment = package.PickupCourierId != null;

            package.PickupCourierId = courierUserId;

            if (package.Status == PackageStatus.Registered)
                package.Status = PackageStatus.PickupAssigned;

            _dbContext.PackageStatusHistories.Add(new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = package.Status,
                Timestamp = DateTime.UtcNow,
                ChangedByUserId = adminId,
                Note = isReassignment
                    ? "The administrator changed the assigned pickup courier."
                    : "The administrator assigned a pickup courier."
            });

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId, int adminId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.", nameof(trackingNumber));

            if (courierUserId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierUserId));

            if (adminId <= 0)
                throw new ArgumentOutOfRangeException(nameof(adminId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
                return false;

            if (!DeliveryReassignableStatuses.Contains(package.Status))
                return false;

            var courierExists = await _dbContext.CourierProfiles
                .AnyAsync(cp => cp.UserId == courierUserId);

            if (!courierExists)
                return false;

            bool isReassignment = package.DeliveryCourierId != null;

            package.DeliveryCourierId = courierUserId;

            if (package.Status == PackageStatus.InWarehouse)
                package.Status = PackageStatus.DeliveryAssigned;

            _dbContext.PackageStatusHistories.Add(new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = package.Status,
                Timestamp = DateTime.UtcNow,
                ChangedByUserId = adminId,
                Note = isReassignment
                    ? "The administrator changed the assigned delivery courier."
                    : "The administrator assigned a delivery courier."
            });

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<CourierOptionResult>> GetCouriersAsync()
        {
            return await _dbContext.CourierProfiles
                .AsNoTracking()
                .Where(cp => cp.User != null)
                .OrderBy(cp => cp.User!.FullName)
                .Select(cp => new CourierOptionResult
                {
                    UserId = cp.UserId,
                    FullName = cp.User!.FullName,
                    VehiclePlateNumber = cp.VehiclePlateNumber
                }).ToListAsync();
        }

        private async Task ApplyStatusTimestampsAsync(Package package, PackageStatus newStatus, DateTime now)
        {
            switch (newStatus)
            {
                case PackageStatus.PickedUp:
                    package.PickedUpAt ??= now;
                    break;

                case PackageStatus.InWarehouse:
                case PackageStatus.DeliveryAssigned:
                    package.PickedUpAt ??= now;
                    package.ReceivedInWarehouseAt ??= now;
                    package.WarehouseId ??= await _dbContext.Warehouses
                        .Select(w => (int?)w.Id)
                        .FirstOrDefaultAsync();
                    break;

                case PackageStatus.OutForDelivery:
                    package.OutForDeliveryAt ??= now;
                    break;

                case PackageStatus.Delivered:
                    package.OutForDeliveryAt ??= now;
                    package.DeliveredAt ??= now;
                    break;
            }
        }

        public async Task<List<AdminUserResult>> GetUsersAsync(string? role)
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .Select(u => new AdminUserResult
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber,
                    Role = (from ur in _dbContext.UserRoles
                            join r in _dbContext.Roles on ur.RoleId equals r.Id
                            where ur.UserId == u.Id
                            select r.Name).FirstOrDefault() ?? string.Empty,
                    VehiclePlateNumber = u.CourierProfile != null ? u.CourierProfile.VehiclePlateNumber : null,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(role))
                users = users.Where(u => u.Role == role).ToList();

            return users;
        }

        public async Task<AdminUserResult?> GetUserAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new AdminUserResult
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber,
                    Role = (from ur in _dbContext.UserRoles
                            join r in _dbContext.Roles on ur.RoleId equals r.Id
                            where ur.UserId == u.Id
                            select r.Name).FirstOrDefault() ?? string.Empty,
                    VehiclePlateNumber = u.CourierProfile != null ? u.CourierProfile.VehiclePlateNumber : null,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateStaffUserAsync(CreateStaffUserRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (!StaffRoles.Contains(request.Role))
                return false;

            if (request.Role == UserRoles.Courier && string.IsNullOrWhiteSpace(request.VehiclePlateNumber))
                return false;

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return false;

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Staff user creation failed. Email: {Email}, Errors: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
            }

            if (request.Role == UserRoles.Courier)
            {
                _dbContext.CourierProfiles.Add(new CourierProfile
                {
                    UserId = user.Id,
                    VehiclePlateNumber = request.VehiclePlateNumber!.Trim()
                });

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Staff user created by admin. Email: {Email}, Role: {Role}", request.Email, request.Role);

            return true;
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var user = await _userManager.Users
                .Include(u => u.CourierProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            var emailOwner = await _userManager.FindByEmailAsync(request.Email);
            if (emailOwner != null && emailOwner.Id != user.Id)
                return false;

            // Seeded / admin-created users use the e-mail as user name as well
            if (string.Equals(user.UserName, user.Email, StringComparison.OrdinalIgnoreCase))
                user.UserName = request.Email;

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User update failed. UserId: {UserId}, Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            var isCourier = await _userManager.IsInRoleAsync(user, UserRoles.Courier);

            if (isCourier && !string.IsNullOrWhiteSpace(request.VehiclePlateNumber))
            {
                if (user.CourierProfile == null)
                {
                    _dbContext.CourierProfiles.Add(new CourierProfile
                    {
                        UserId = user.Id,
                        VehiclePlateNumber = request.VehiclePlateNumber.Trim()
                    });
                }
                else
                {
                    user.CourierProfile.VehiclePlateNumber = request.VehiclePlateNumber.Trim();
                }

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("User updated by admin. UserId: {UserId}", userId);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            if (userId == adminId)
                return false;

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // A courier with active packages cannot be deleted.
            var hasActiveAssignment = await _dbContext.Packages.AnyAsync(p =>
                (p.PickupCourierId == userId &&
                    (p.Status == PackageStatus.PickupAssigned || p.Status == PackageStatus.PickedUp)) ||
                (p.DeliveryCourierId == userId &&
                    (p.Status == PackageStatus.DeliveryAssigned || p.Status == PackageStatus.OutForDelivery)));

            if (hasActiveAssignment)
            {
                _logger.LogWarning("User deletion rejected, courier has active packages. UserId: {UserId}", userId);
                return false;
            }

            // Package foreign keys are 'Restrict', so they must be cleared first.
            var relatedPackages = await _dbContext.Packages
                .Where(p => p.SenderUserId == userId ||
                            p.RecipientUserId == userId ||
                            p.PickupCourierId == userId ||
                            p.DeliveryCourierId == userId)
                .ToListAsync();

            foreach (var package in relatedPackages)
            {
                if (package.SenderUserId == userId) package.SenderUserId = null;
                if (package.RecipientUserId == userId) package.RecipientUserId = null;
                if (package.PickupCourierId == userId) package.PickupCourierId = null;
                if (package.DeliveryCourierId == userId) package.DeliveryCourierId = null;
            }

            await _dbContext.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning("User deletion failed. UserId: {UserId}, Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("User deleted by admin. UserId: {UserId}, AdminId: {AdminId}", userId, adminId);

            return true;
        }

        public async Task<AdminStatisticsResult> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var periodStart = startDate.Date;
            var periodEnd = endDate.Date.AddDays(1);

            if (periodStart >= periodEnd)
                throw new ArgumentException("The start date must be earlier than the end date.");

            var dayCount = (periodEnd - periodStart).Days;

            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Select(p => new PackageStatisticsRow
                {
                    Status = p.Status,
                    WarehouseId = p.WarehouseId,
                    PickupCourierId = p.PickupCourierId,
                    DeliveryCourierId = p.DeliveryCourierId,
                    CreatedAt = p.CreatedAt,
                    PickedUpAt = p.PickedUpAt,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt,
                    OutForDeliveryAt = p.OutForDeliveryAt,
                    DeliveredAt = p.DeliveredAt
                })
                .ToListAsync();

            var warehouses = await _dbContext.Warehouses
                .AsNoTracking()
                .Select(w => new { w.Id, w.Name, w.MaxCapacity })
                .ToListAsync();

            var couriers = await GetCouriersAsync();

            var deliveredInPeriod = packages
                .Where(p => IsInPeriod(p.DeliveredAt, periodStart, periodEnd))
                .ToList();

            var result = new AdminStatisticsResult
            {
                StartDate = periodStart,
                EndDate = periodEnd.AddDays(-1),
                TotalPackageCount = packages.Count,
                ActivePackageCount = packages.Count(p =>
                    p.Status != PackageStatus.Delivered &&
                    p.Status != PackageStatus.Cancelled),
                DeliveredInPeriodCount = deliveredInPeriod.Count,
                AverageDeliveryTimeHours = AverageHours(deliveredInPeriod, p => p.OutForDeliveryAt, p => p.DeliveredAt),
                AverageTotalTransitTimeHours = AverageHours(deliveredInPeriod, p => p.PickedUpAt ?? p.CreatedAt, p => p.DeliveredAt)
            };

            // Stored packages at the end of each day, calculated from the timestamps
            foreach (var warehouse in warehouses)
            {
                var warehousePackages = packages.Where(p => p.WarehouseId == warehouse.Id).ToList();
                var currentCount = warehousePackages.Count(p => InWarehouseStatuses.Contains(p.Status));

                var history = new List<WarehouseUtilizationPointResult>();

                for (var day = periodStart; day < periodEnd; day = day.AddDays(1))
                {
                    var dayEnd = day.AddDays(1);

                    var count = warehousePackages.Count(p =>
                        p.Status != PackageStatus.Cancelled &&
                        p.ReceivedInWarehouseAt != null &&
                        p.ReceivedInWarehouseAt < dayEnd &&
                        (p.LeftWarehouseAt == null || p.LeftWarehouseAt >= dayEnd));

                    history.Add(new WarehouseUtilizationPointResult
                    {
                        Date = DateOnly.FromDateTime(day),
                        PackageCount = count,
                        UtilizationPercent = Percent(count, warehouse.MaxCapacity)
                    });
                }

                result.Warehouses.Add(new WarehouseStatisticsResult
                {
                    WarehouseId = warehouse.Id,
                    WarehouseName = warehouse.Name,
                    MaxCapacity = warehouse.MaxCapacity,
                    CurrentPackageCount = currentCount,
                    CurrentUtilizationPercent = Percent(currentCount, warehouse.MaxCapacity),
                    PeakPackageCount = history.Count > 0 ? history.Max(h => h.PackageCount) : 0,
                    AverageUtilizationPercent = history.Count > 0 ? Math.Round(history.Average(h => h.UtilizationPercent), 1) : 0,
                    History = history
                });
            }

            foreach (var courier in couriers)
            {
                var pickups = packages.Count(p =>
                    p.PickupCourierId == courier.UserId &&
                    IsInPeriod(p.PickedUpAt, periodStart, periodEnd));

                var delivered = deliveredInPeriod
                    .Where(p => p.DeliveryCourierId == courier.UserId)
                    .ToList();

                var activeAssignments = packages.Count(p =>
                    (p.PickupCourierId == courier.UserId &&
                        (p.Status == PackageStatus.PickupAssigned || p.Status == PackageStatus.PickedUp)) ||
                    (p.DeliveryCourierId == courier.UserId &&
                        (p.Status == PackageStatus.DeliveryAssigned || p.Status == PackageStatus.OutForDelivery)));

                result.Couriers.Add(new CourierStatisticsResult
                {
                    CourierUserId = courier.UserId,
                    FullName = courier.FullName,
                    VehiclePlateNumber = courier.VehiclePlateNumber,
                    PickupCount = pickups,
                    DeliveredCount = delivered.Count,
                    AveragePackagesPerDay = Math.Round((double)(pickups + delivered.Count) / dayCount, 2),
                    AverageDeliveryTimeHours = AverageHours(delivered, p => p.OutForDeliveryAt, p => p.DeliveredAt),
                    ActiveAssignmentCount = activeAssignments
                });
            }

            for (var day = periodStart; day < periodEnd; day = day.AddDays(1))
            {
                var dayEnd = day.AddDays(1);

                result.DailyCounts.Add(new DailyPackageCountResult
                {
                    Date = DateOnly.FromDateTime(day),
                    CreatedCount = packages.Count(p => p.CreatedAt >= day && p.CreatedAt < dayEnd),
                    PickedUpCount = packages.Count(p => IsInPeriod(p.PickedUpAt, day, dayEnd)),
                    DeliveredCount = packages.Count(p => IsInPeriod(p.DeliveredAt, day, dayEnd))
                });
            }

            return result;
        }

        private static bool IsInPeriod(DateTime? value, DateTime start, DateTime end)
            => value != null && value >= start && value < end;

        private static double Percent(int count, int maxCapacity)
            => maxCapacity <= 0 ? 0 : Math.Round(count * 100.0 / maxCapacity, 1);

        private static double? AverageHours(
            IEnumerable<PackageStatisticsRow> rows,
            Func<PackageStatisticsRow, DateTime?> from,
            Func<PackageStatisticsRow, DateTime?> to)
        {
            var durations = rows
                .Where(r => from(r) != null && to(r) != null && to(r) >= from(r))
                .Select(r => (to(r)!.Value - from(r)!.Value).TotalHours)
                .ToList();

            return durations.Count == 0 ? null : Math.Round(durations.Average(), 1);
        }

        private class PackageStatisticsRow
        {
            public PackageStatus Status { get; set; }
            public int? WarehouseId { get; set; }
            public int? PickupCourierId { get; set; }
            public int? DeliveryCourierId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? PickedUpAt { get; set; }
            public DateTime? ReceivedInWarehouseAt { get; set; }
            public DateTime? OutForDeliveryAt { get; set; }
            public DateTime? DeliveredAt { get; set; }

            // A package leaves the warehouse when the courier starts the delivery
            public DateTime? LeftWarehouseAt => OutForDeliveryAt ?? DeliveredAt;
        }
    }
}
