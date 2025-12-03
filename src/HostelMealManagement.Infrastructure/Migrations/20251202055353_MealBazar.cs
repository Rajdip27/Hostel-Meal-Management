using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MealBazar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealBazars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BazarDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    BazarAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealBazars", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "372b60b1-f1d9-47da-ac9e-33fe84d389ea", "AQAAAAIAAYagAAAAEKxGAd0lNK5f3hCBN9DBpSfaByZ8XnwZD8ryQ60tStM0NlK+frbXY4xygJRkDtIBqw==", "18bae5ae-1331-4db8-a1e7-e67c35b34aaa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "047f2827-6453-4144-95e7-c2b960871fb6", "AQAAAAIAAYagAAAAELN0nZBfzaf9KyTW6j3Ppo8lCi/tA+237Gpmgrl47v/Av13vDsrusmvfK/vJ1KDwzg==", "82492f03-3b9e-4b4c-9222-657795be1057" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealBazars");

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
    }
}
