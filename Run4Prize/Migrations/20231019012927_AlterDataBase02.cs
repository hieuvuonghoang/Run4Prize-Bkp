using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Run4Prize.Migrations
{
    /// <inheritdoc />
    public partial class AlterDataBase02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "DistanceTarget",
                table: "Weeks",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceTarget",
                table: "Weeks");
        }
    }
}
