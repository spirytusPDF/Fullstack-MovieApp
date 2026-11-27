using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AdminRolePopulated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("0af665c5-ed06-4e2c-8440-ad5cbc8e104f"), new Guid("45dafa4b-7fc6-4286-4aa4-08df4359550c") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0af665c5-ed06-4e2c-8440-ad5cbc8e104f"), new Guid("45dafa4b-7fc6-4286-4aa4-08df4359550c") });
        }
    }
}
