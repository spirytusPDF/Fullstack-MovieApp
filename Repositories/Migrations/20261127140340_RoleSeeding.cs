using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RoleSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0af665c5-ed06-4e2c-8440-ad5cbc8e104f"), "0AF665C5-ED06-4E2C-8440-AD5CBC8E104F", "Role for admins who have full access to data", "Admin", "ADMIN" },
                    { new Guid("81aa91f1-4f6f-4e02-ac03-e0f64706c1d2"), "81AA91F1-4F6F-4E02-AC03-E0F64706C1D2", "Role for costumers and users without the ability to affect data", "User", "USER" },
                    { new Guid("a0a6e690-6cfb-47d8-a259-a86eda667c16"), "A0A6E690-6CFB-47D8-A259-A86EDA667C16", "Role for managers who can affect data only with permission", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0af665c5-ed06-4e2c-8440-ad5cbc8e104f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("81aa91f1-4f6f-4e02-ac03-e0f64706c1d2"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a0a6e690-6cfb-47d8-a259-a86eda667c16"));

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "11111111-1111-1111-1111-111111111111", "Admin", "ADMIN" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "22222222-2222-2222-2222-222222222222", "User", "USER" }
                });
        }
    }
}
