using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWebApplicationResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebApplicationResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourcePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    BuildCommand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartupCommand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthCheckUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebApplicationResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebApplicationResources_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebApplicationConfigurationEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebApplicationResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebApplicationConfigurationEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebApplicationConfigurationEntries_WebApplicationResources_WebApplicationResourceId",
                        column: x => x.WebApplicationResourceId,
                        principalTable: "WebApplicationResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebApplicationConfigurationEntries_WebApplicationResourceId",
                table: "WebApplicationConfigurationEntries",
                column: "WebApplicationResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WebApplicationResources_CreatedAt",
                table: "WebApplicationResources",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebApplicationResources_Name",
                table: "WebApplicationResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebApplicationResources_ResourceGroupId",
                table: "WebApplicationResources",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WebApplicationResources_UpdatedAt",
                table: "WebApplicationResources",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebApplicationConfigurationEntries");

            migrationBuilder.DropTable(
                name: "WebApplicationResources");
        }
    }
}
