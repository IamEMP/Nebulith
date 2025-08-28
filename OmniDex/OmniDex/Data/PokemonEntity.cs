using System.ComponentModel.DataAnnotations;

namespace OmniDex.Data;

public class PokemonEntity
{
    [Key] // This makes Id the primary key
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Details that can be fetched later
    public int? Height { get; set; }
    public int? Weight { get; set; }
    public string? ImageUrl { get; set; }
    public string? TypesJson { get; set; } // We'll store the list of types as a JSON string

    public string? EvolutionChainIds { get; set; }

    public int GenerationId { get; set; }

    // For the 2-week refresh logic
    public DateTime LastUpdated { get; set; }

}
