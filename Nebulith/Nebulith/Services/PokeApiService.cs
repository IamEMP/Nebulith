using Microsoft.Extensions.Caching.Memory;
using Nebulith.Models;
using Microsoft.Extensions.Logging;

namespace Nebulith.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://pokeapi.co/api/v2";
        private readonly IMemoryCache _cache;
        private readonly ILogger<PokeApiService> _logger = null!;

        public PokeApiService(HttpClient httpClient, IMemoryCache cache, ILogger<PokeApiService> logger)
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

        public async Task<List<EvolutionStage>?> GetEvolutionChainAsync(string pokemonName)
        {
            var cacheKey = $"evolution-details-{pokemonName.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out List<EvolutionStage>? cachedChain))
            {
                return cachedChain;
            }

            try
            {
                var speciesUrl = $"{BaseUrl}/pokemon-species/{pokemonName.ToLower()}";
                var speciesData = await _httpClient.GetFromJsonAsync<PokemonSpecies>(speciesUrl);
                if (speciesData == null || string.IsNullOrEmpty(speciesData.EvolutionChain.Url)) return null;

                var evolutionData = await _httpClient.GetFromJsonAsync<EvolutionChainResponse>(speciesData.EvolutionChain.Url);
                if (evolutionData == null) return null;

                var evolutionChain = new List<EvolutionStage>();
                var currentLink = evolutionData.Chain;

                // Use a recursive helper function to parse the chain
                ParseChain(currentLink, evolutionChain);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(30));
                _cache.Set(cacheKey, evolutionChain, cacheEntryOptions);

                return evolutionChain;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching evolution chain for {PokemonName}", pokemonName);
                return null;
            }
        }

        // This helper function recursively walks the chain and formats the trigger text
        private void ParseChain(ChainLink link, List<EvolutionStage> stages)
        {
            if (link == null) return;

            stages.Add(new EvolutionStage
            {
                Pokemon = link.Species,
                // The first Pokémon in the chain has no trigger, but its evolutions do.
                TriggerText = FormatEvolutionTrigger(link.EvolutionDetails.FirstOrDefault())
            });

            foreach (var nextLink in link.EvolvesTo)
            {
                ParseChain(nextLink, stages);
            }
        }

        // This helper creates the user-friendly text like "Level 16" or "Use Fire Stone"
        private string FormatEvolutionTrigger(EvolutionDetail? detail)
        {
            if (detail == null) return string.Empty;

            return detail.Trigger.Name.Replace("-", " ") switch
            {
                "level up" when detail.MinLevel.HasValue => $"Level {detail.MinLevel}",
                "use item" when detail.Item != null => $"Use {detail.Item.Name.Replace("-", " ")}",
                "trade" => "Trade",
                _ => $"By {detail.Trigger.Name.Replace("-", " ")}"
            };
        }
        public async Task<List<PokemonResult>?> GetAllPokemonListAsync()
        {
            // --- FOR DEVELOPMENT ---
            // Fetches only Generation 1 Pokémon for fast database seeding.
            var response = await _httpClient.GetFromJsonAsync<PokemonListResponse>($"{BaseUrl}/pokemon?limit=151");

            // --- FOR PRODUCTION ---
            // Uncomment this line when you are ready to seed the full database.
            // var response = await _httpClient.GetFromJsonAsync<PokemonListResponse>($"{BaseUrl}/pokemon?limit=1500");

            return response?.Results;
        }

        public async Task<PokemonSpecies?> GetPokemonSpeciesAsync(string name)
        {
            var cacheKey = $"species-{name.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out PokemonSpecies? cachedSpecies))
            {
                return cachedSpecies;
            }

            try
            {
                var species = await _httpClient.GetFromJsonAsync<PokemonSpecies>($"{BaseUrl}/pokemon-species/{name.ToLower()}");
                if (species != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
                    _cache.Set(cacheKey, species, cacheEntryOptions);
                }
                return species;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }

}