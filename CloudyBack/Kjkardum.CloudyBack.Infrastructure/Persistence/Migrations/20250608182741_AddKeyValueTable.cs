using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyValueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyValueTableEntries",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValueTableEntries", x => x.Key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyValueTableEntries");
        }
    }
}
