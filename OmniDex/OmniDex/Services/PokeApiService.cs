using OmniDex.Models;

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

        public async Task<List<Client.Services.PokeService.Result>> GetPokemonListAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<Client.Services.PokeService.PokemonListResponse>($"{BaseUrl}pokemon?limit=151");
            return response?.Results ?? new List<Client.Services.PokeService.Result>();
        }

        public async Task<PokemonDetail?> GetPokemonDetailsAsync(string name)
        {
            try
            {
                var url = $"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}";
                var response = await _httpClient.GetFromJsonAsync<PokemonDetail>(url);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching details for {name}: {ex.Message}");
                return null;
            }

        }


    }
}
