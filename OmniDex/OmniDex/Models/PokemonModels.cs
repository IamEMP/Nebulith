using System;
using System.Text.Json.Serialization;

namespace OmniDex.Models
{
    // Add these to PokemonModels.cs

    public class PokemonListResponse
    {
        [JsonPropertyName("results")]
        public List<PokemonResult> Results { get; set; } = new();
    }

    public class PokemonResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class PokemonDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("height")]
        public int Height { get; set; } // In decimetres

        [JsonPropertyName("weight")]
        public int Weight { get; set; } // In hectograms

        [JsonPropertyName("sprites")]
        public SpriteInfo Sprites { get; set; } = new();

        [JsonPropertyName("types")]
        public List<TypeInfo> Types { get; set; } = [];
    }

    public class SpriteInfo
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; } = string.Empty;
    }

    public class TypeInfo
    {
        [JsonPropertyName("type")]
        public TypeName Type { get; set; } = new();
    }

    public class TypeName
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class PokemonSpecies
    {
        [JsonPropertyName("evolution_chain")]
        public EvolutionChainUrl EvolutionChain { get; set; } = new();

        [JsonPropertyName("generation")]
        public GenerationInfo Generation { get; set; } = new();
    }

    public class EvolutionChainUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    // For the evolution-chain endpoint URL, to get the actual chain data
    public class EvolutionChainResponse
    {
        [JsonPropertyName("chain")]
        public ChainLink Chain { get; set; } = new();
    }

    // This class is recursive to handle the nested structure
    public class ChainLink
    {
        [JsonPropertyName("species")]
        public PokemonResult Species { get; set; } = new(); // We can reuse PokemonResult here!

        [JsonPropertyName("evolves_to")]
        public List<ChainLink> EvolvesTo { get; set; } = new();
    }

    public class GenerationInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
