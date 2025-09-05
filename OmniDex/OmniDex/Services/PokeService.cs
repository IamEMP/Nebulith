namespace OmniDex.Client.Services;

public class PokeService
{
    
    public class PokemonListResponse
    {
        public int Count { get; set; }
        public required List<Result> Results { get; set; }
    }

    public class Result
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
    }


    public class PokemonDetail
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Sprites Sprites { get; set; }
        public required List<TypeElement> Types { get; set; }
        
    }

    public class Sprites
    {
        public required string Front_default { get; set; }
        // Note: The API uses snake_case, but C# convention is PascalCase.
        // Use [JsonPropertyName("front_default")] if you want to use PascalCase property names.
        // public string FrontDefault { get; set; }
    }

    public class TypeElement
    {
        public required TypeClass Type { get; set; }
    }

    public class TypeClass
    {
        public required string Name { get; set; }
    }
}