using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gotorz.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "TravelPackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TravelPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TravelPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualPrice",
                table: "TravelPackages",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceMarkupPercentage",
                table: "TravelPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "ManualPrice",
                table: "TravelPackages");

            migrationBuilder.DropColumn(
                name: "PriceMarkupPercentage",
                table: "TravelPackages");
        }
    }
}
