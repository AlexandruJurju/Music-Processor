using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedHashFromSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata_hash",
                table: "songs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "metadata_hash",
                table: "songs",
                type: "TEXT",
                maxLength: 44,
                nullable: false,
                defaultValue: "");
        }
    }
}
