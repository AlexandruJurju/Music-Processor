using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genre_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    remove_from_songs = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    artist_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "genre_genre_category",
                columns: table => new
                {
                    genre_categories_id = table.Column<int>(type: "INTEGER", nullable: false),
                    genres_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_genre_category", x => new { x.genre_categories_id, x.genres_id });
                    table.ForeignKey(
                        name: "fk_genre_genre_category_genre_categories_genre_categories_id",
                        column: x => x.genre_categories_id,
                        principalTable: "genre_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_genre_genre_category_genres_genres_id",
                        column: x => x.genres_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    isrc = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    date = table.Column<int>(type: "INTEGER", nullable: true),
                    track_number = table.Column<int>(type: "INTEGER", nullable: false),
                    tracks_count = table.Column<int>(type: "INTEGER", nullable: false),
                    disc_number = table.Column<int>(type: "INTEGER", nullable: false),
                    disc_count = table.Column<int>(type: "INTEGER", nullable: false),
                    duration = table.Column<int>(type: "INTEGER", nullable: false),
                    album_id = table.Column<int>(type: "INTEGER", nullable: true),
                    main_artist_id = table.Column<int>(type: "INTEGER", nullable: false),
                    spotify_info_spotify_song_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    spotify_info_spotify_song_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    spotify_info_spotify_cover_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    spotify_info_spotify_album_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    spotify_info_spotify_artist_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_songs", x => x.id);
                    table.ForeignKey(
                        name: "fk_songs_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_songs_artists_main_artist_id",
                        column: x => x.main_artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "song_artists",
                columns: table => new
                {
                    artists_id = table.Column<int>(type: "INTEGER", nullable: false),
                    songs_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_song_artists", x => new { x.artists_id, x.songs_id });
                    table.ForeignKey(
                        name: "fk_song_artists_artists_artists_id",
                        column: x => x.artists_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_song_artists_songs_songs_id",
                        column: x => x.songs_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "ix_albums_artist_id",
                table: "albums",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_artists_name",
                table: "artists",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_genre_categories_name",
                table: "genre_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_genre_genre_category_genres_id",
                table: "genre_genre_category",
                column: "genres_id");

            migrationBuilder.CreateIndex(
                name: "ix_genres_name",
                table: "genres",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_song_artists_songs_id",
                table: "song_artists",
                column: "songs_id");

            migrationBuilder.CreateIndex(
                name: "ix_song_genres_songs_id",
                table: "song_genres",
                column: "songs_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_album_id",
                table: "songs",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_main_artist_id",
                table: "songs",
                column: "main_artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_name",
                table: "songs",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genre_genre_category");

            migrationBuilder.DropTable(
                name: "song_artists");

            migrationBuilder.DropTable(
                name: "song_genres");

            migrationBuilder.DropTable(
                name: "genre_categories");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "artists");
        }
    }
}
