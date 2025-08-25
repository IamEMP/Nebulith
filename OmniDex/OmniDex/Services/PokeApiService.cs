using static OmniDex.Client.Services.PokeService;

namespace OmniDex.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://pokeapi.co/api/v2/";

        public PokeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Result>> GetPokemonListAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<PokemonListResponse>($"{BaseUrl}pokemon?limit=151");
            return response?.Results ?? new List<Result>();
        }

        public async Task<PokemonDetail?> GetPokemonDetailsAsync(string name)
        {
            return await _httpClient.GetFromJsonAsync<PokemonDetail>($"{BaseUrl}pokemon/{name}");
        }
    }
}
