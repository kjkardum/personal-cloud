using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKafkaClusterResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KafkaClusterResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KafkaClusterResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KafkaClusterResources_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KafkaClusterResources_CreatedAt",
                table: "KafkaClusterResources",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_KafkaClusterResources_Name",
                table: "KafkaClusterResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KafkaClusterResources_ResourceGroupId",
                table: "KafkaClusterResources",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_KafkaClusterResources_UpdatedAt",
                table: "KafkaClusterResources",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KafkaClusterResources");
        }
    }
}
