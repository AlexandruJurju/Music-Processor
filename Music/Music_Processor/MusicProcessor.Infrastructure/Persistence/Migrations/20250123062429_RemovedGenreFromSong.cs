using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedGenreFromSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "song_genres");

            migrationBuilder.AddColumn<int>(
                name: "genre_id",
                table: "songs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_songs_genre_id",
                table: "songs",
                column: "genre_id");

            migrationBuilder.AddForeignKey(
                name: "fk_songs_genres_genre_id",
                table: "songs",
                column: "genre_id",
                principalTable: "genres",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_songs_genres_genre_id",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_genre_id",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "genre_id",
                table: "songs");

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
                name: "ix_song_genres_songs_id",
                table: "song_genres",
                column: "songs_id");
        }
    }
}
