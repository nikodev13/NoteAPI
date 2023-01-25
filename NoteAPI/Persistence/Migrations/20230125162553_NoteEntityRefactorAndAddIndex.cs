using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NoteEntityRefactorAndAddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Notes",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Id_Title",
                table: "Notes",
                columns: new[] { "Id", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_Id_Title",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notes",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
