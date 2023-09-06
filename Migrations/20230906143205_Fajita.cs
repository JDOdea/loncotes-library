using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    public partial class Fajita : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "Paid", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 6, new DateTime(2023, 8, 10, 15, 40, 34, 37, DateTimeKind.Unspecified).AddTicks(7880), 13, false, 4, null },
                    { 7, new DateTime(2023, 8, 16, 15, 40, 34, 37, DateTimeKind.Unspecified).AddTicks(7880), 5, false, 3, null },
                    { 8, new DateTime(2023, 8, 12, 15, 40, 34, 37, DateTimeKind.Unspecified).AddTicks(7880), 6, false, 1, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
