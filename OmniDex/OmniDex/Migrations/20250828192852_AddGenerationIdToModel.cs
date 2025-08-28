using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniDex.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationIdToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "Pokemons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "Pokemons");
        }
    }
}
