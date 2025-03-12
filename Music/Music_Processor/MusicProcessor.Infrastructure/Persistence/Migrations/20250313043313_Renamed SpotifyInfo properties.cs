using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedSpotifyInfoproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "spotify_info_song_url",
                table: "songs",
                newName: "spotify_info_spotify_song_url");

            migrationBuilder.RenameColumn(
                name: "spotify_info_song_id",
                table: "songs",
                newName: "spotify_info_spotify_song_id");

            migrationBuilder.RenameColumn(
                name: "spotify_info_cover_url",
                table: "songs",
                newName: "spotify_info_spotify_cover_url");

            migrationBuilder.RenameColumn(
                name: "spotify_info_artist_id",
                table: "songs",
                newName: "spotify_info_spotify_artist_id");

            migrationBuilder.RenameColumn(
                name: "spotify_info_album_id",
                table: "songs",
                newName: "spotify_info_spotify_album_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "spotify_info_spotify_song_url",
                table: "songs",
                newName: "spotify_info_song_url");

            migrationBuilder.RenameColumn(
                name: "spotify_info_spotify_song_id",
                table: "songs",
                newName: "spotify_info_song_id");

            migrationBuilder.RenameColumn(
                name: "spotify_info_spotify_cover_url",
                table: "songs",
                newName: "spotify_info_cover_url");

            migrationBuilder.RenameColumn(
                name: "spotify_info_spotify_artist_id",
                table: "songs",
                newName: "spotify_info_artist_id");

            migrationBuilder.RenameColumn(
                name: "spotify_info_spotify_album_id",
                table: "songs",
                newName: "spotify_info_album_id");
        }
    }
}
