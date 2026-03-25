using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingWithStopPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DropoffPointId",
                table: "Bookings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PickupPointId",
                table: "Bookings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DropoffPointId",
                table: "Bookings",
                column: "DropoffPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PickupPointId",
                table: "Bookings",
                column: "PickupPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StopPoints_DropoffPointId",
                table: "Bookings",
                column: "DropoffPointId",
                principalTable: "StopPoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StopPoints_PickupPointId",
                table: "Bookings",
                column: "PickupPointId",
                principalTable: "StopPoints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StopPoints_DropoffPointId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StopPoints_PickupPointId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DropoffPointId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PickupPointId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DropoffPointId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PickupPointId",
                table: "Bookings");
        }
    }
}
