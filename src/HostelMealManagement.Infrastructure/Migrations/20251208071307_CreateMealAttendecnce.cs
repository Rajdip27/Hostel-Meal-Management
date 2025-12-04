using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateMealAttendecnce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealAttendances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    MealDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsBreakfast = table.Column<bool>(type: "bit", nullable: false),
                    IsLunch = table.Column<bool>(type: "bit", nullable: false),
                    IsDinner = table.Column<bool>(type: "bit", nullable: false),
                    IsGuest = table.Column<bool>(type: "bit", nullable: false),
                    GuestIsBreakfast = table.Column<bool>(type: "bit", nullable: false),
                    GuestBreakfastQty = table.Column<int>(type: "int", nullable: false),
                    GuestIsLunch = table.Column<bool>(type: "bit", nullable: false),
                    GuestLunchQty = table.Column<int>(type: "int", nullable: false),
                    GuestIsDinner = table.Column<bool>(type: "bit", nullable: false),
                    GuestDinnerQty = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealAttendances_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_MealAttendances_MemberId",
                table: "MealAttendances",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealAttendances");

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
        }
    }
}
