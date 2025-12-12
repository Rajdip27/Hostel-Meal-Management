using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMealManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Cratedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1f406c3c-1579-4496-b8eb-60edb0bcd908", "AQAAAAIAAYagAAAAEA07IxcqJzyj3xSj3PdB5E4k2cjt3NNehRft4Z8uX4/SX/foBEPjSgXbKWIQYcF2Ng==", "cdc34853-10f1-471c-a42a-c11a5a69639a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c3f896fe-3c23-425f-80f3-38284961e2da", "Manager@localhost.com", "MANAGER@LOCALHOST.COM", "MANAGER@LOCALHOST.COM", "AQAAAAIAAYagAAAAEBTyX72cV1BCPUg634lr1M2pG3TNFZcdEusKIDG36l4yMneHh9TOxYeOdBO5rY0msw==", "0ac7e589-0091-461d-a0fd-d759d16777bb", "Manager@localhost.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "8ae5e813-2755-40bf-af07-663b8bbc2edd", "employee@localhost.com", "EMPLOYEE@LOCALHOST.COM", "EMPLOYEE@LOCALHOST.COM", "AQAAAAIAAYagAAAAEG00zpsGIhPsj4o0wZIB0l+ZIc6ny6JoAuvtVhBNNShCqOGdvfTHdGEr+7trPZ/qXQ==", "204b4053-4e01-4886-975f-e648152bbb70", "employee@localhost.com" });
        }
    }
}
