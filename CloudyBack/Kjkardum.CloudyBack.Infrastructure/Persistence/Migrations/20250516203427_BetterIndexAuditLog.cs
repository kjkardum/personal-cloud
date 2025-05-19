using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BetterIndexAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_ResourceId",
                table: "AuditLogEntries");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ResourceId_Timestamp",
                table: "AuditLogEntries",
                columns: new[] { "ResourceId", "Timestamp" },
                unique: true,
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_ResourceId_Timestamp",
                table: "AuditLogEntries");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ResourceId",
                table: "AuditLogEntries",
                column: "ResourceId");
        }
    }
}
