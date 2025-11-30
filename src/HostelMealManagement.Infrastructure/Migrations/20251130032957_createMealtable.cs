using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createMealtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealCycle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealCycle", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "90a985a0-c1f6-4a96-927a-284857212843", "AQAAAAIAAYagAAAAEBUMLb8K7W7wyvZrqRvqggZw7cWQz9iPvtCbiYD3w+d1pTqzstna65SJvzhA4GyavA==", "ee38c9b8-79a8-41ca-9fb1-cc0d71b43270" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "83be1bd6-87f4-4684-a0b4-18138c450c1f", "AQAAAAIAAYagAAAAECPb92y+M38JuHXtt9JSI5qR7z4kd0zfuG2kauWdyv4yNanawcdRaCgq+7mLpj6HEg==", "deba2279-1ed9-4c7c-99c2-f84321730578" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealCycle");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "56ee446f-b35f-408a-b38c-dcb3c6ad3cb9", "AQAAAAIAAYagAAAAEBldgMVS+odxoI8nslMWuh+dtiBbG9ryMQecD/nuwQdDMY3jdohnJve+FtAAaULTvA==", "5e134bf6-2fec-4b16-b107-da8a28ebebc7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19bbe0c7-696c-4d96-96e3-ba40910856c1", "AQAAAAIAAYagAAAAEMm6xceV16t28lDy4vqh1+TbJWY4q5Sq3IYCGUNQzYOs1XzfxIMz1nyQvqfKYQ9A6g==", "bbaef947-be55-42b9-89ab-0339aeb02564" });
        }
    }
}
