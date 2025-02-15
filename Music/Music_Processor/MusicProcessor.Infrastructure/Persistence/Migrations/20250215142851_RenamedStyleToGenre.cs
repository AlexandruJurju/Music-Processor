using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedStyleToGenre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_genre_styles_genres_genres_id",
                table: "genre_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_styles_styles_styles_id",
                table: "genre_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_song_styles_styles_styles_id",
                table: "song_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_genres_genre_id",
                table: "songs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_song_styles",
                table: "song_styles");

            migrationBuilder.DropIndex(
                name: "ix_song_styles_styles_id",
                table: "song_styles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_genre_styles",
                table: "genre_styles");

            migrationBuilder.DropIndex(
                name: "ix_genre_styles_styles_id",
                table: "genre_styles");

            migrationBuilder.RenameColumn(
                name: "genre_id",
                table: "songs",
                newName: "genre_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_songs_genre_id",
                table: "songs",
                newName: "ix_songs_genre_category_id");

            migrationBuilder.RenameColumn(
                name: "styles_id",
                table: "song_styles",
                newName: "genres_id");

            migrationBuilder.RenameColumn(
                name: "styles_id",
                table: "genre_styles",
                newName: "genre_categories_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_song_styles",
                table: "song_styles",
                columns: new[] { "genres_id", "songs_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_genre_styles",
                table: "genre_styles",
                columns: new[] { "genre_categories_id", "genres_id" });

            migrationBuilder.CreateIndex(
                name: "ix_song_styles_songs_id",
                table: "song_styles",
                column: "songs_id");

            migrationBuilder.CreateIndex(
                name: "ix_genre_styles_genres_id",
                table: "genre_styles",
                column: "genres_id");

            migrationBuilder.AddForeignKey(
                name: "fk_genre_styles_genres_genre_categories_id",
                table: "genre_styles",
                column: "genre_categories_id",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_styles_styles_genres_id",
                table: "genre_styles",
                column: "genres_id",
                principalTable: "styles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_song_styles_styles_genres_id",
                table: "song_styles",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_genre_styles_genres_genre_categories_id",
                table: "genre_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_styles_styles_genres_id",
                table: "genre_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_song_styles_styles_genres_id",
                table: "song_styles");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_genres_genre_category_id",
                table: "songs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_song_styles",
                table: "song_styles");

            migrationBuilder.DropIndex(
                name: "ix_song_styles_songs_id",
                table: "song_styles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_genre_styles",
                table: "genre_styles");

            migrationBuilder.DropIndex(
                name: "ix_genre_styles_genres_id",
                table: "genre_styles");

            migrationBuilder.RenameColumn(
                name: "genre_category_id",
                table: "songs",
                newName: "genre_id");

            migrationBuilder.RenameIndex(
                name: "ix_songs_genre_category_id",
                table: "songs",
                newName: "ix_songs_genre_id");

            migrationBuilder.RenameColumn(
                name: "genres_id",
                table: "song_styles",
                newName: "styles_id");

            migrationBuilder.RenameColumn(
                name: "genre_categories_id",
                table: "genre_styles",
                newName: "styles_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_song_styles",
                table: "song_styles",
                columns: new[] { "songs_id", "styles_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_genre_styles",
                table: "genre_styles",
                columns: new[] { "genres_id", "styles_id" });

            migrationBuilder.CreateIndex(
                name: "ix_song_styles_styles_id",
                table: "song_styles",
                column: "styles_id");

            migrationBuilder.CreateIndex(
                name: "ix_genre_styles_styles_id",
                table: "genre_styles",
                column: "styles_id");

            migrationBuilder.AddForeignKey(
                name: "fk_genre_styles_genres_genres_id",
                table: "genre_styles",
                column: "genres_id",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_styles_styles_styles_id",
                table: "genre_styles",
                column: "styles_id",
                principalTable: "styles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_song_styles_styles_styles_id",
                table: "song_styles",
                column: "styles_id",
                principalTable: "styles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_songs_genres_genre_id",
                table: "songs",
                column: "genre_id",
                principalTable: "genres",
                principalColumn: "id");
        }
    }
}
