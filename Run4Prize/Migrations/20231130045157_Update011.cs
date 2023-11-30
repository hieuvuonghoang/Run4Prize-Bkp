using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Run4Prize.Migrations
{
    /// <inheritdoc />
    public partial class Update011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "Activities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MovingTime",
                table: "Activities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "MovingTime",
                table: "Activities");
        }
    }
}
