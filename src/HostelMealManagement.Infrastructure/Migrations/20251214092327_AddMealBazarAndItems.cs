using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealBazarAndItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MealBazars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "MealBazars",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "MealMemberId",
                table: "MealBazars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "MealBazars",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "TotalDays",
                table: "MealBazars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3ca494be-904f-4a90-a5d7-1fe855eb5f54", "AQAAAAIAAYagAAAAEOc16F89bZeZpvpxGDkioOWaT0VYAlz0VDpHurXWv/aTM1KULs60rheaWm72BfRXNA==", "9205aa0c-e585-46de-bdad-c91e9d98aa77" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be717eb8-3c8b-40fa-8d66-fa8971080317", "AQAAAAIAAYagAAAAEMll7wI1ok9nBHEvp5de93HYjkTn6it8FptvZNYSTgRlzW/AfQ+tOT5DHrztfj7yHQ==", "16e0a2fb-2833-43c2-b7b9-d85c0d9838d4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MealBazars");

            migrationBuilder.DropColumn(
                name: "MealMemberId",
                table: "MealBazars");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "MealBazars");

            migrationBuilder.DropColumn(
                name: "TotalDays",
                table: "MealBazars");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MealBazars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2def778f-223b-4721-9e31-e194ac2ec24c", "AQAAAAIAAYagAAAAEGpVtk1mbdKqZLQiH2iy8+li06n3lSDs8R+VkHD5/hM/CodvukukNZNwHT/HYwJptg==", "4ca60af8-77e2-451a-9651-cf38afd9e72a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "62f2e35f-83bd-4301-a873-a2233b2740da", "AQAAAAIAAYagAAAAEN8S2dngWTZAubwqSpwgQ4Ue0q+MSD9gCFRpaQKREVW6XCSK9KIuZy2U48zwAT4q1A==", "5499778d-f44a-4419-ba07-c9969a8ad0a0" });
        }
    }
}
