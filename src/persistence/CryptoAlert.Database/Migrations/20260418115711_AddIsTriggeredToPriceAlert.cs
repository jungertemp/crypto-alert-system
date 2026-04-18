using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoAlert.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTriggeredToPriceAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTriggered",
                table: "PriceAlerts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTriggered",
                table: "PriceAlerts");
        }
    }
}
