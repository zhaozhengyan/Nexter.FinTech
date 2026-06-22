using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexter.Fintech.Database.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalItemsJsonAndSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalItemsJson",
                table: "Items",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalItemsJson",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Items");
        }
    }
}
