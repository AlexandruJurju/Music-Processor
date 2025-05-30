using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Database.Migrations
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
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "styles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    soft_deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_styles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    main_artist_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_artists_main_artist_id",
                        column: x => x.main_artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre_style",
                columns: table => new
                {
                    genres_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    style_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_style", x => new { x.genres_id, x.style_id });
                    table.ForeignKey(
                        name: "fk_genre_style_genres_genres_id",
                        column: x => x.genres_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_genre_style_styles_style_id",
                        column: x => x.style_id,
                        principalTable: "styles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    key = table.Column<string>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    main_artist_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    album_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    disc_number = table.Column<int>(type: "INTEGER", nullable: false),
                    disc_count = table.Column<int>(type: "INTEGER", nullable: false),
                    duration = table.Column<int>(type: "INTEGER", nullable: false),
                    year = table.Column<uint>(type: "INTEGER", nullable: false),
                    track_number = table.Column<int>(type: "INTEGER", nullable: false),
                    tracks_count = table.Column<int>(type: "INTEGER", nullable: false),
                    isrc = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    spotify_metadata_spotify_song_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    spotify_metadata_spotify_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    spotify_metadata_spotify_cover_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    spotify_metadata_spotify_album_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    spotify_metadata_spotify_artist_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artist_song",
                columns: table => new
                {
                    artists_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    song_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artist_song", x => new { x.artists_id, x.song_id });
                    table.ForeignKey(
                        name: "fk_artist_song_artists_artists_id",
                        column: x => x.artists_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artist_song_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_style",
                columns: table => new
                {
                    song_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    styles_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_song_style", x => new { x.song_id, x.styles_id });
                    table.ForeignKey(
                        name: "fk_song_style_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_song_style_styles_styles_id",
                        column: x => x.styles_id,
                        principalTable: "styles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_albums_main_artist_id",
                table: "albums",
                column: "main_artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_name",
                table: "albums",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_artist_song_song_id",
                table: "artist_song",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "ix_artists_name",
                table: "artists",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_genre_style_style_id",
                table: "genre_style",
                column: "style_id");

            migrationBuilder.CreateIndex(
                name: "ix_genres_name",
                table: "genres",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_song_style_styles_id",
                table: "song_style",
                column: "styles_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_album_id",
                table: "songs",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_isrc",
                table: "songs",
                column: "isrc");

            migrationBuilder.CreateIndex(
                name: "ix_songs_key",
                table: "songs",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "ix_songs_main_artist_id",
                table: "songs",
                column: "main_artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_title",
                table: "songs",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_songs_year",
                table: "songs",
                column: "year");

            migrationBuilder.CreateIndex(
                name: "ix_styles_name",
                table: "styles",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artist_song");

            migrationBuilder.DropTable(
                name: "genre_style");

            migrationBuilder.DropTable(
                name: "song_style");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "styles");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "artists");
        }
    }
}
