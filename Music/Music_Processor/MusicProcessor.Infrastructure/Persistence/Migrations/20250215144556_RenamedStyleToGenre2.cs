using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedStyleToGenre2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_genre_genre_category_genres_genre_categories_id",
                table: "genre_genre_category");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_genre_category_styles_genres_id",
                table: "genre_genre_category");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_genres_genre_category_id",
                table: "songs");

            migrationBuilder.DropTable(
                name: "song_styles");

            migrationBuilder.DropTable(
                name: "styles");

            migrationBuilder.AddColumn<bool>(
                name: "remove_from_songs",
                table: "genres",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "genre_categories",
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
                    table.PrimaryKey("pk_genre_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "song_genres",
                columns: table => new
                {
                    genres_id = table.Column<int>(type: "INTEGER", nullable: false),
                    songs_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_song_genres", x => new { x.genres_id, x.songs_id });
                    table.ForeignKey(
                        name: "fk_song_genres_genres_genres_id",
                        column: x => x.genres_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_song_genres_songs_songs_id",
                        column: x => x.songs_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_genre_categories_name",
                table: "genre_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_song_genres_songs_id",
                table: "song_genres",
                column: "songs_id");

            migrationBuilder.AddForeignKey(
                name: "fk_genre_genre_category_genre_categories_genre_categories_id",
                table: "genre_genre_category",
                column: "genre_categories_id",
                principalTable: "genre_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_genre_category_genres_genres_id",
                table: "genre_genre_category",
                column: "genres_id",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_songs_genre_categories_genre_category_id",
                table: "songs",
                column: "genre_category_id",
                principalTable: "genre_categories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_genre_genre_category_genre_categories_genre_categories_id",
                table: "genre_genre_category");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_genre_category_genres_genres_id",
                table: "genre_genre_category");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_genre_categories_genre_category_id",
                table: "songs");

            migrationBuilder.DropTable(
                name: "genre_categories");

            migrationBuilder.DropTable(
                name: "song_genres");

            migrationBuilder.DropColumn(
                name: "remove_from_songs",
                table: "genres");

            migrationBuilder.CreateTable(
                name: "styles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    remove_from_songs = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_styles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "song_styles",
                columns: table => new
                {
                    genres_id = table.Column<int>(type: "INTEGER", nullable: false),
                    songs_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_song_styles", x => new { x.genres_id, x.songs_id });
                    table.ForeignKey(
                        name: "fk_song_styles_songs_songs_id",
                        column: x => x.songs_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_song_styles_styles_genres_id",
                        column: x => x.genres_id,
                        principalTable: "styles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_song_styles_songs_id",
                table: "song_styles",
                column: "songs_id");

            migrationBuilder.CreateIndex(
                name: "ix_styles_name",
                table: "styles",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_genre_category_genres_genre_categories_id",
                table: "genre_genre_category",
                column: "genre_categories_id",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_genre_category_styles_genres_id",
                table: "genre_genre_category",
                column: "genres_id",
                principalTable: "styles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_songs_genres_genre_category_id",
                table: "songs",
                column: "genre_category_id",
                principalTable: "genres",
                principalColumn: "id");
        }
    }
}
