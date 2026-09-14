using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehiclePlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false, defaultValue: 500),
                    Address_BuildingDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_PostalCode = table.Column<int>(type: "int", nullable: false),
                    Address_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_StreetNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackingNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    ScheduledPickupDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientUserId = table.Column<int>(type: "int", nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupCourierId = table.Column<int>(type: "int", nullable: true),
                    DeliveryCourierId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedInWarehouseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutForDeliveryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryAddress_BuildingDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_PostalCode = table.Column<int>(type: "int", nullable: false),
                    DeliveryAddress_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_StreetNumber = table.Column<int>(type: "int", nullable: false),
                    SenderAddress_BuildingDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress_PostalCode = table.Column<int>(type: "int", nullable: false),
                    SenderAddress_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress_StreetNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Users_DeliveryCourierId",
                        column: x => x.DeliveryCourierId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Users_PickupCourierId",
                        column: x => x.PickupCourierId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseCapacityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPackageCount = table.Column<int>(type: "int", nullable: false),
                    MaxCapacitySnapshot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseCapacityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseCapacityLogs_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DeliveryCourierId",
                table: "Packages",
                column: "DeliveryCourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PickupCourierId",
                table: "Packages",
                column: "PickupCourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_RecipientUserId",
                table: "Packages",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ScheduledPickupDate",
                table: "Packages",
                column: "ScheduledPickupDate");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_SenderUserId",
                table: "Packages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Status",
                table: "Packages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_TrackingNumber",
                table: "Packages",
                column: "TrackingNumber",
                unique: true,
                filter: "[TrackingNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_WarehouseId",
                table: "Packages",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_ChangedByUserId",
                table: "PackageStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_PackageId",
                table: "PackageStatusHistories",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_Timestamp",
                table: "PackageStatusHistories",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCapacityLogs_LoggedAt",
                table: "WarehouseCapacityLogs",
                column: "LoggedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCapacityLogs_WarehouseId",
                table: "WarehouseCapacityLogs",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageStatusHistories");

            migrationBuilder.DropTable(
                name: "WarehouseCapacityLogs");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Warehouses");
        }
    }
}
