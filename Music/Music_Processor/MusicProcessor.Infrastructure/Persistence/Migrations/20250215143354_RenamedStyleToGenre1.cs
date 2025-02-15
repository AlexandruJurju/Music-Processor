using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedStyleToGenre1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genre_styles");

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
                        name: "fk_genre_genre_category_genres_genre_categories_id",
                        column: x => x.genre_categories_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_genre_genre_category_styles_genres_id",
                        column: x => x.genres_id,
                        principalTable: "styles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_genre_genre_category_genres_id",
                table: "genre_genre_category",
                column: "genres_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genre_genre_category");

            migrationBuilder.CreateTable(
                name: "genre_styles",
                columns: table => new
                {
                    genre_categories_id = table.Column<int>(type: "INTEGER", nullable: false),
                    genres_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_styles", x => new { x.genre_categories_id, x.genres_id });
                    table.ForeignKey(
                        name: "fk_genre_styles_genres_genre_categories_id",
                        column: x => x.genre_categories_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_genre_styles_styles_genres_id",
                        column: x => x.genres_id,
                        principalTable: "styles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_genre_styles_genres_id",
                table: "genre_styles",
                column: "genres_id");
        }
    }
}
