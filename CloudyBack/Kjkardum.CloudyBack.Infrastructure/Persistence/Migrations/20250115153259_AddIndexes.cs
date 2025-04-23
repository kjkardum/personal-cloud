using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PostgresServerResources_CreatedAt",
                table: "PostgresServerResources",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PostgresServerResources_UpdatedAt",
                table: "PostgresServerResources",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PostgresDatabaseResources_CreatedAt",
                table: "PostgresDatabaseResources",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PostgresDatabaseResources_UpdatedAt",
                table: "PostgresDatabaseResources",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostgresServerResources_CreatedAt",
                table: "PostgresServerResources");

            migrationBuilder.DropIndex(
                name: "IX_PostgresServerResources_UpdatedAt",
                table: "PostgresServerResources");

            migrationBuilder.DropIndex(
                name: "IX_PostgresDatabaseResources_CreatedAt",
                table: "PostgresDatabaseResources");

            migrationBuilder.DropIndex(
                name: "IX_PostgresDatabaseResources_UpdatedAt",
                table: "PostgresDatabaseResources");
        }
    }
}
