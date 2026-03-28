using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProvincesTableAndChangeLocationProvinceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StopPoints_Provinces_ProvinceId",
                table: "StopPoints");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_StopPoints_ProvinceId",
                table: "StopPoints");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "StopPoints");

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "StopPoints",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "StopPoints");

            migrationBuilder.AddColumn<Guid>(
                name: "ProvinceId",
                table: "StopPoints",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StopPoints_ProvinceId",
                table: "StopPoints",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_StopPoints_Provinces_ProvinceId",
                table: "StopPoints",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
