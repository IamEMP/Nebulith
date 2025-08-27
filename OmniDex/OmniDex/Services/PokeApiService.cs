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
        // In Services/PokeApiService.cs

        public async Task<List<PokemonResult>?> GetEvolutionChainAsync(string pokemonName)
        {
            var cacheKey = $"evolution-{pokemonName.ToLower()}";

            // Check the cache first
            if (_cache.TryGetValue(cacheKey, out List<PokemonResult>? cachedChain))
            {
                return cachedChain;
            }

            try
            {
                // Get the species data to find the evolution chain URL
                var speciesUrl = $"{BaseUrl}/pokemon-species/{pokemonName.ToLower()}";
                var speciesData = await _httpClient.GetFromJsonAsync<PokemonSpecies>(speciesUrl);
                if (speciesData == null || string.IsNullOrEmpty(speciesData.EvolutionChain.Url))
                {
                    return null;
                }

                // Get the actual evolution chain data from the URL
                var evolutionData = await _httpClient.GetFromJsonAsync<EvolutionChainResponse>(speciesData.EvolutionChain.Url);
                if (evolutionData == null)
                {
                    return null;
                }

                // Process the nested data into a flat list
                var evolutionChain = new List<PokemonResult>();
                var currentLink = evolutionData.Chain;
                do
                {
                    evolutionChain.Add(currentLink.Species);
                    currentLink = currentLink.EvolvesTo.FirstOrDefault();
                } while (currentLink != null);

                // Cache the result
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKey, evolutionChain, cacheEntryOptions);

                return evolutionChain;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching evolution chain for {pokemonName}: {ex.Message}");
                return null;
            }
        }
    }
}