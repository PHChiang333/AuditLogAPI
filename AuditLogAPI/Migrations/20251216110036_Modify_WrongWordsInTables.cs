using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogAPI.Migrations
{
    /// <inheritdoc />
    public partial class Modify_WrongWordsInTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "ApiLogs",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Products",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ApiLogs",
                newName: "CreateAt");
        }
    }
}
