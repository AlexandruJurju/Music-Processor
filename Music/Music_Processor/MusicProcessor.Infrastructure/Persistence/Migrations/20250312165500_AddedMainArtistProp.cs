using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedMainArtistProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "main_artist_id",
                table: "songs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_songs_main_artist_id",
                table: "songs",
                column: "main_artist_id");

            migrationBuilder.AddForeignKey(
                name: "fk_songs_artists_main_artist_id",
                table: "songs",
                column: "main_artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_songs_artists_main_artist_id",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_main_artist_id",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "main_artist_id",
                table: "songs");
        }
    }
}
