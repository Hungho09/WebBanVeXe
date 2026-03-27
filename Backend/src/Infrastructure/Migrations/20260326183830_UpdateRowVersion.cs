using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Seats",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$.N14FFcQMoCcN3OZvl3DkOMgSok1ocYfR2qyWMxVmlpuqUxxl20la");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Seats");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices",
                column: "BookingId");
        }
    }
}
