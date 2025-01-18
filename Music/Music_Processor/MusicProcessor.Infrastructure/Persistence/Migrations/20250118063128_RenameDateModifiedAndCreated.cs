using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateModifiedAndCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Styles",
                newName: "DateModified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Styles",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Songs",
                newName: "DateModified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Songs",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Genres",
                newName: "DateModified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Genres",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Artists",
                newName: "DateModified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Artists",
                newName: "DateCreated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "Styles",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Styles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "Songs",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Songs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "Genres",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Genres",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "Artists",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Artists",
                newName: "CreatedAt");
        }
    }
}
