using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArch_Products.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInformationDocumentToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InformationDocument",
                table: "Products",
                type: "nvarchar(2050)",
                maxLength: 2050,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InformationDocument",
                table: "Products");
        }
    }
}
