using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MealMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealMenu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    MealName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MenuItems = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealMenu", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "affdc34f-8bde-4b0f-960e-d093c2cdeec0", "AQAAAAIAAYagAAAAELsn90Y/dWr73TW9xMSD/fyQ3l4CUywbJRsJsfhGKZu/y5ybEk8/NTRBmm6pXfoTGg==", "f0e0f9ae-417a-45b2-92d7-c8195e518801" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8ae5e813-2755-40bf-af07-663b8bbc2edd", "AQAAAAIAAYagAAAAEG00zpsGIhPsj4o0wZIB0l+ZIc6ny6JoAuvtVhBNNShCqOGdvfTHdGEr+7trPZ/qXQ==", "204b4053-4e01-4886-975f-e648152bbb70" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealMenu");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9290228d-b8b9-4043-b1e3-e885e3717024", "AQAAAAIAAYagAAAAEL2YU8+J16eIq67VLCAWiBpACZXNq6WgT2FRQzbnqzsEvrkgkX2iH/TBdKixMtKCKA==", "c3c3a0fa-a0ec-46d8-bd18-d11d7614c5f0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "48b727c9-608b-478a-82dc-fbf2b13433d3", "AQAAAAIAAYagAAAAEEdv4ggInRIhs80i00g/fvz11Te0Xhf9Cp5qeAvDf/Q0bjeSbubmL9L2i8n8JHoJ0A==", "8aa070ed-dbcf-4ec8-afa3-ad92a6431fe8" });
        }
    }
}
