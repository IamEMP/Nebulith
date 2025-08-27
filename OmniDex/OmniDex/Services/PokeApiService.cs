using Microsoft.Extensions.Caching.Memory;
using OmniDex.Models;

namespace OmniDex.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://pokeapi.co/api/v2";
        private readonly IMemoryCache _cache;

        public PokeApiService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<PokemonResult>?> GetPokemonListAsync()
        {
            const string cacheKey = "pokemon-list";

            if (_cache.TryGetValue(cacheKey, out List<PokemonResult>? cachedList))
            {
                return cachedList;
            }

            
            var response = await _httpClient.GetFromJsonAsync<PokemonListResponse>($"{BaseUrl}/pokemon?limit=151");
            var pokemonList = response?.Results;

            if (pokemonList != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKey, pokemonList, cacheEntryOptions);
            }

            return pokemonList;
        }

        public async Task<PokemonDetail?> GetPokemonDetailsAsync(string name)
        {
            var cacheKey = $"pokemon-{name.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out PokemonDetail? cachedDetail))
            {
                return cachedDetail;
            }

            
            try
            {
               
                var url = $"{BaseUrl}/pokemon/{name.ToLower()}";

                
                var pokemonDetail = await _httpClient.GetFromJsonAsync<PokemonDetail>(url);

                if (pokemonDetail != null)
                {
                    
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                    _cache.Set(cacheKey, pokemonDetail, cacheEntryOptions);
                }

                return pokemonDetail;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching details for {name}: {ex.Message}");
                return null;
            }
        }
    }
}