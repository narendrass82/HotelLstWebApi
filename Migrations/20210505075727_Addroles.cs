using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelLstWebApi.Migrations
{
    public partial class Addroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8a614a3b-fb47-4430-bf0e-7c585a0dc6a5", "0ef960b8-c188-41cd-a265-424bb354a3b0", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9a7a1083-d9a4-4004-adeb-bbf5a4b9c4b4", "cc5550a2-64b7-43a5-9713-e3cc9744550c", "Administrator", "ADMINSTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a614a3b-fb47-4430-bf0e-7c585a0dc6a5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a7a1083-d9a4-4004-adeb-bbf5a4b9c4b4");
        }
    }
}
