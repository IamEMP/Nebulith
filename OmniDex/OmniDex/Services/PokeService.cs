namespace OmniDex.Client.Services;

public class PokeService
{
    // Models/PokemonListResponse.cs
    public class PokemonListResponse
    {
        public int Count { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

// Models/PokemonDetail.cs
    public class PokemonDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Sprites Sprites { get; set; }
        public List<TypeElement> Types { get; set; }
        // Add other properties like Stats, Height, Weight, etc.
    }

    public class Sprites
    {
        public string Front_default { get; set; }
        // Note: The API uses snake_case, but C# convention is PascalCase.
        // Use [JsonPropertyName("front_default")] if you want to use PascalCase property names.
        // public string FrontDefault { get; set; }
    }

    public class TypeElement
    {
        public TypeClass Type { get; set; }
    }

    public class TypeClass
    {
        public string Name { get; set; }
    }
}