using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniDex.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelAfterContextFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvolutionChainIds",
                table: "Pokemons",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvolutionChainIds",
                table: "Pokemons");
        }
    }
}
