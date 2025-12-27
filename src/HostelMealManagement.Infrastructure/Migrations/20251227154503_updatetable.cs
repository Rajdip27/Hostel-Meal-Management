using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBill",
                table: "MealBill",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GasBill",
                table: "MealBill",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServantBill",
                table: "MealBill",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "92b8e480-378e-44a9-bfc2-448420479f47", "AQAAAAIAAYagAAAAECnvv7LUw81I8BE7iyJHIl3LwGBIxu8hwu4IGCrMAFtzM0KXJAw+q+hl08PKjD6AWg==", "2b8c0a00-3887-4f28-a305-5064e38b3fae" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38343dad-4b8f-4f94-98db-a2cb0a943d1e", "AQAAAAIAAYagAAAAEM0ZpyWUlNg2srjBWOj+cBbuJi7oF+vKO054suVpgPA6hCMyN//b3LmUCo7+FOarlQ==", "30681df4-1756-41b0-a900-41ed7a502c76" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBill",
                table: "MealBill");

            migrationBuilder.DropColumn(
                name: "GasBill",
                table: "MealBill");

            migrationBuilder.DropColumn(
                name: "ServantBill",
                table: "MealBill");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be510705-a8b2-4af0-acef-1e7c2c41ea71", "AQAAAAIAAYagAAAAEPBeVg42OeqZeOziE6HfCiFwNULKq67xt1RgGy3WX9LTn9BEdhpUE6/k5HAMPbN3+A==", "f6d4bdcc-c21a-4389-a41c-e2d4b44030a1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7129ae62-f455-4354-ae32-00638b1d4848", "AQAAAAIAAYagAAAAEEKbDq5ZrzcouDa3mJbrc+e536m3X2/XKefSJ0Nze8kJkzUiZ2JIVyfdJYwZsRhixQ==", "7272f086-d110-4036-8cad-ca669cf65ab5" });
        }
    }
}
