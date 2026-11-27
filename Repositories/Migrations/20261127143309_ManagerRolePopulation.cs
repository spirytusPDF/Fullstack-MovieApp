using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class ManagerRolePopulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("a0a6e690-6cfb-47d8-a259-a86eda667c16"), new Guid("d0d47afa-af49-4d57-42b6-08df434c9869") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("a0a6e690-6cfb-47d8-a259-a86eda667c16"), new Guid("d0d47afa-af49-4d57-42b6-08df434c9869") });
        }
    }
}
