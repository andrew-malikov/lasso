using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(20,10)", precision: 20, scale: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies");
        }
    }
}
