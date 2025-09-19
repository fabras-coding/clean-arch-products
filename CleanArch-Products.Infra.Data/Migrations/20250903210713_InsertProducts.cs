using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArch_Products.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.Sql("SET IDENTITY_INSERT Products ON");
            migrationBuilder.Sql("INSERT INTO Products (Id, Name, Description, Price, Stock, Image, CategoryId) VALUES (1, 'Caderno', 'Caderno universitário 10 matérias', 25.50, 100, 'caderno.jpg', 1)");
            migrationBuilder.Sql("INSERT INTO Products (Id, Name, Description, Price, Stock, Image, CategoryId) VALUES (2, 'Caneta', 'Caneta esferográfica azul', 2.30, 500, 'caneta.jpg', 1)");
            migrationBuilder.Sql("INSERT INTO Products (Id, Name, Description, Price, Stock, Image, CategoryId) VALUES (3, 'Notebook', 'Notebook Dell Inspiron 15', 3500.00, 50, 'notebook.jpg', 2)");
            migrationBuilder.Sql("INSERT INTO Products (Id, Name, Description, Price, Stock, Image, CategoryId) VALUES (4, 'Mouse', 'Mouse sem fio Logitech', 150.75, 200, 'mouse.jpg', 3)");
            migrationBuilder.Sql("SET IDENTITY_INSERT Products OFF");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
