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

        public List<FlavorTextEntry> FlavorTexts { get; set; } = new();
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

        [JsonPropertyName("flavor_text_entries")]
        public List<FlavorTextEntry> FlavorTextEntries { get; set; } = new();
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
        public PokemonResult Species { get; set; } = new();

        [JsonPropertyName("evolves_to")]
        public List<ChainLink> EvolvesTo { get; set; } = new();

        [JsonPropertyName("evolution_details")]
        public List<EvolutionDetail> EvolutionDetails { get; set; } = new();
    }

    public class GenerationInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }


    public class EvolutionDetail
    {
        [JsonPropertyName("min_level")]
        public int? MinLevel { get; set; }

        [JsonPropertyName("item")]
        public ItemInfo? Item { get; set; }

        [JsonPropertyName("trigger")]
        public TriggerInfo Trigger { get; set; } = new();
    }

    public class ItemInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TriggerInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class EvolutionStage
    {
        public PokemonResult Pokemon { get; set; } = new();
        public string TriggerText { get; set; } = string.Empty;
    }
    public class FlavorTextEntry
    {
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public LanguageInfo Language { get; set; } = new();

        [JsonPropertyName("version")]
        public VersionInfo Version { get; set; } = new();
    }

    public class LanguageInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class VersionInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    

}
