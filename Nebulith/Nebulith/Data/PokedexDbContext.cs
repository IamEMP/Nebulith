

using Microsoft.EntityFrameworkCore;

namespace Nebulith.Data;

public class PokedexDbContext : DbContext
{
    public DbSet<PokemonEntity> Pokemons { get; set; }
    public PokedexDbContext(DbContextOptions<PokedexDbContext> options) : base(options) { }

}
