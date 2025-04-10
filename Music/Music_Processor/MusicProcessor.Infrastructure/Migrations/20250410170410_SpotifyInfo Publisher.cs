using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SpotifyInfoPublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "spotify_info_spotify_publisher",
                table: "songs",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "spotify_info_spotify_publisher",
                table: "songs");
        }
    }
}
