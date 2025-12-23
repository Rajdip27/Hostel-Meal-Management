using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillYearMonthDateToUtilityBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UtilityBill_UtilityType",
                table: "UtilityBill");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BillDate",
                table: "UtilityBill",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "BillMonth",
                table: "UtilityBill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillYear",
                table: "UtilityBill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bdc965ef-7b1c-4da0-964a-dd4576b1a857", "AQAAAAIAAYagAAAAEP9bIqrhxSKL0+l6bwUgzeSaCKXsy4i51ZzPyCxFGD84kpT0xiSB7eYJwSuH9/6QdQ==", "69a9fdce-f8f2-4fee-9290-c97cd2090836" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c05a68f1-6e66-42f1-8c4f-a933b2de6b0d", "AQAAAAIAAYagAAAAENg4dUs5x6jDubFwRnjh5lcRnUHuC4dTq7H6ouQc2wWmlQnqfUnlPUKbNDqqfbyedQ==", "7548e8f0-7395-4adf-ac06-07a13140162e" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityBill_BillYear_BillMonth",
                table: "UtilityBill",
                columns: new[] { "BillYear", "BillMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityBill_UtilityType_BillYear_BillMonth",
                table: "UtilityBill",
                columns: new[] { "UtilityType", "BillYear", "BillMonth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UtilityBill_BillYear_BillMonth",
                table: "UtilityBill");

            migrationBuilder.DropIndex(
                name: "IX_UtilityBill_UtilityType_BillYear_BillMonth",
                table: "UtilityBill");

            migrationBuilder.DropColumn(
                name: "BillDate",
                table: "UtilityBill");

            migrationBuilder.DropColumn(
                name: "BillMonth",
                table: "UtilityBill");

            migrationBuilder.DropColumn(
                name: "BillYear",
                table: "UtilityBill");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffcbe84f-0cf1-4dd9-8b9e-2a20c5f165f8", "AQAAAAIAAYagAAAAEMEin1AZWF19pQbPZFD3qK/2KLLe4M9CAeng33ATXTda/gXy8fW+M7dPG+0dl4982Q==", "83661d95-c2f0-4ebf-8243-74ee791629e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc7fa116-b277-4427-8167-c2062e075b3a", "AQAAAAIAAYagAAAAEBX1GI4psCNn/yCe/qIDNRCC64lsslMbdcZGF4kBpXfk0U0vQMsK0Ke31K+C85MkBA==", "c0e61b10-8809-4e5a-81f1-2f32f4578a60" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityBill_UtilityType",
                table: "UtilityBill",
                column: "UtilityType");
        }
    }
}
