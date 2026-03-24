using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SeatTemplates",
                columns: new[] { "Id", "BusType", "ColumnNumber", "Floor", "RowNumber", "SeatNumber", "Type" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), 0, 1, 1, 1, "A01", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000002"), 0, 2, 1, 1, "A02", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000003"), 0, 3, 1, 1, "A03", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000004"), 0, 1, 1, 2, "A04", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000005"), 0, 2, 1, 2, "A05", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000006"), 0, 3, 1, 2, "A06", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000007"), 0, 1, 1, 3, "A07", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000008"), 0, 2, 1, 3, "A08", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000009"), 0, 3, 1, 3, "A09", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000010"), 0, 1, 1, 4, "A10", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000011"), 0, 2, 1, 4, "A11", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000012"), 0, 3, 1, 4, "A12", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000013"), 0, 1, 1, 5, "A13", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000014"), 0, 2, 1, 5, "A14", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000015"), 0, 3, 1, 5, "A15", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000016"), 0, 1, 1, 6, "A16", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000017"), 0, 2, 1, 6, "A17", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000018"), 0, 3, 1, 6, "A18", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000019"), 0, 1, 2, 1, "B01", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000020"), 0, 2, 2, 1, "B02", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000021"), 0, 3, 2, 1, "B03", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000022"), 0, 1, 2, 2, "B04", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000023"), 0, 2, 2, 2, "B05", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000024"), 0, 3, 2, 2, "B06", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000025"), 0, 1, 2, 3, "B07", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000026"), 0, 2, 2, 3, "B08", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000027"), 0, 3, 2, 3, "B09", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000028"), 0, 1, 2, 4, "B10", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000029"), 0, 2, 2, 4, "B11", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000030"), 0, 3, 2, 4, "B12", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000031"), 0, 1, 2, 5, "B13", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000032"), 0, 2, 2, 5, "B14", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000033"), 0, 3, 2, 5, "B15", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000034"), 0, 1, 2, 6, "B16", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000035"), 0, 2, 2, 6, "B17", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000036"), 0, 3, 2, 6, "B18", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000001"), 1, 1, 1, 1, "S01", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000002"), 1, 2, 1, 1, "S02", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000003"), 1, 3, 1, 1, "S03", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000004"), 1, 4, 1, 1, "S04", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000005"), 1, 5, 1, 1, "S05", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000006"), 1, 1, 1, 2, "S06", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000007"), 1, 2, 1, 2, "S07", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000008"), 1, 3, 1, 2, "S08", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000009"), 1, 4, 1, 2, "S09", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000010"), 1, 5, 1, 2, "S10", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000011"), 1, 1, 1, 3, "S11", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000012"), 1, 2, 1, 3, "S12", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000013"), 1, 3, 1, 3, "S13", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000014"), 1, 4, 1, 3, "S14", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000015"), 1, 5, 1, 3, "S15", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000016"), 1, 1, 1, 4, "S16", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000017"), 1, 2, 1, 4, "S17", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000018"), 1, 3, 1, 4, "S18", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000019"), 1, 4, 1, 4, "S19", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000020"), 1, 5, 1, 4, "S20", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000021"), 1, 1, 1, 5, "S21", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000022"), 1, 2, 1, 5, "S22", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000023"), 1, 3, 1, 5, "S23", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000024"), 1, 4, 1, 5, "S24", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000025"), 1, 5, 1, 5, "S25", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000026"), 1, 1, 1, 6, "S26", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000027"), 1, 2, 1, 6, "S27", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000028"), 1, 3, 1, 6, "S28", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000029"), 1, 4, 1, 6, "S29", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000030"), 1, 5, 1, 6, "S30", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000031"), 1, 1, 1, 7, "S31", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000032"), 1, 2, 1, 7, "S32", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000033"), 1, 3, 1, 7, "S33", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000034"), 1, 4, 1, 7, "S34", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000035"), 1, 5, 1, 7, "S35", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000036"), 1, 1, 1, 8, "S36", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000037"), 1, 2, 1, 8, "S37", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000038"), 1, 3, 1, 8, "S38", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000039"), 1, 4, 1, 8, "S39", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000040"), 1, 5, 1, 8, "S40", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000041"), 1, 1, 1, 9, "S41", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000042"), 1, 2, 1, 9, "S42", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000043"), 1, 3, 1, 9, "S43", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000044"), 1, 4, 1, 9, "S44", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000045"), 1, 5, 1, 9, "S45", 0 },
                    { new Guid("90000000-0000-0000-0000-000000000001"), 2, 1, 1, 1, "L01", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000002"), 2, 2, 1, 1, "L02", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000003"), 2, 3, 1, 1, "L03", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000004"), 2, 1, 1, 2, "L04", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000005"), 2, 2, 1, 2, "L05", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000006"), 2, 3, 1, 2, "L06", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000007"), 2, 1, 1, 3, "L07", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000008"), 2, 2, 1, 3, "L08", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000009"), 2, 3, 1, 3, "L09", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000038"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000039"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000040"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000041"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000042"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000043"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000044"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("45000000-0000-0000-0000-000000000045"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "SeatTemplates",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000009"));
        }
    }
}
