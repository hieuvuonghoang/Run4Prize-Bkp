using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Run4Prize.Migrations
{
    /// <inheritdoc />
    public partial class Update09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Activitys",
                table: "Activitys");

            migrationBuilder.RenameTable(
                name: "Activitys",
                newName: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_Activitys_MemberId",
                table: "Activities",
                newName: "IX_Activities_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Activitys_CreateDate",
                table: "Activities",
                newName: "IX_Activities_CreateDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                table: "Activities",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "Activitys");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_MemberId",
                table: "Activitys",
                newName: "IX_Activitys_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_CreateDate",
                table: "Activitys",
                newName: "IX_Activitys_CreateDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activitys",
                table: "Activitys",
                column: "Id");
        }
    }
}
