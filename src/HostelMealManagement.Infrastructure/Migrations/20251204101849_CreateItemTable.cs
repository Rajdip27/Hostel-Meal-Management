using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealBazarItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealBazarId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealBazarItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealBazarItem_MealBazars_MealBazarId",
                        column: x => x.MealBazarId,
                        principalTable: "MealBazars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "48d806c3-a5a3-4874-b749-747a3ccfd99c", "AQAAAAIAAYagAAAAEK5Uyy/MHMHuMSsdQObk1v8nK3127QxLSW+8jR9QojchVbHL1arc0U35bLCl1OU13w==", "5a4dec29-98a3-4add-8ba7-cbff8c44f1f9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ceb5c26f-d349-4e59-930a-a8860d92a191", "AQAAAAIAAYagAAAAED8uIV8HQoLEJ8ZUCS498HW74lftin/Z3VUwXGzury1229SLlhQlV3pTNJwZozypDQ==", "e7dd62cf-2ace-47b8-9905-8efedd143ead" });

            migrationBuilder.CreateIndex(
                name: "IX_MealBazarItem_MealBazarId",
                table: "MealBazarItem",
                column: "MealBazarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealBazarItem");

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
    }
}
