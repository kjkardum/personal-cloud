using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicProxyConfigurationsToNetworkableResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicProxyConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UseHttps = table.Column<bool>(type: "bit", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    VirtualNetworkableBaseResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicProxyConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicProxyConfigurations_VirtualNetworkableBaseResourceId",
                table: "PublicProxyConfigurations",
                column: "VirtualNetworkableBaseResourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicProxyConfigurations");
        }
    }
}
