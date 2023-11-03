using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Run4Prize.Migrations
{
    /// <inheritdoc />
    public partial class Update01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Settings",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: true);
        }
    }
}
