using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedAlbumEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "album",
                table: "songs");

            migrationBuilder.AddColumn<int>(
                name: "album_id",
                table: "songs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_songs_album_id",
                table: "songs",
                column: "album_id");

            migrationBuilder.AddForeignKey(
                name: "fk_songs_albums_album_id",
                table: "songs",
                column: "album_id",
                principalTable: "albums",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_songs_albums_album_id",
                table: "songs");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropIndex(
                name: "ix_songs_album_id",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "album_id",
                table: "songs");

            migrationBuilder.AddColumn<string>(
                name: "album",
                table: "songs",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
