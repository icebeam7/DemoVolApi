using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoVolApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Title", "Year" },
                values: new object[] { 1, "James Bond", 2020 });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Title", "Year" },
                values: new object[] { 2, "Untamed", 2021 });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Title", "Year" },
                values: new object[] { 3, "Azure Developer", 2022 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
