using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VirtualNetworks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VirtualNetworkResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualNetworkResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualNetworkResources_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VirtualNetworkConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VirtualNetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualNetworkConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualNetworkConnections_VirtualNetworkResources_VirtualNetworkId",
                        column: x => x.VirtualNetworkId,
                        principalTable: "VirtualNetworkResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkConnections_ResourceId",
                table: "VirtualNetworkConnections",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkConnections_VirtualNetworkId",
                table: "VirtualNetworkConnections",
                column: "VirtualNetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkResources_CreatedAt",
                table: "VirtualNetworkResources",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkResources_Name",
                table: "VirtualNetworkResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkResources_ResourceGroupId",
                table: "VirtualNetworkResources",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNetworkResources_UpdatedAt",
                table: "VirtualNetworkResources",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VirtualNetworkConnections");

            migrationBuilder.DropTable(
                name: "VirtualNetworkResources");
        }
    }
}
