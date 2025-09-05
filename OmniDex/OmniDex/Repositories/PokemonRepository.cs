using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OmniDex.Data;
using OmniDex.Models;
using OmniDex.Services;
using System.Text.Json;

namespace OmniDex.Repositories;

public class PokemonRepository
{
    private readonly PokedexDbContext _dbContext;
    private readonly PokeApiService _apiService;
    private readonly ILogger<PokemonRepository> _logger;

    public PokemonRepository(PokedexDbContext dbContext, PokeApiService apiService, ILogger<PokemonRepository> logger)
    {
        _dbContext = dbContext;
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<PokemonEntity>> GetPokemonListAsync()
    {
        return await _dbContext.Pokemons.OrderBy(p => p.Id).ToListAsync();
    }

    public async Task SeedDatabaseAsync()
    {
        if (await _dbContext.Pokemons.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Database is empty. Seeding all Pokémon from the API. This will take several minutes...");
        var allPokemonList = await _apiService.GetAllPokemonListAsync();
        if (allPokemonList == null)
        {
            _logger.LogError("Failed to fetch master Pokémon list from API.");
            return;
        }

        var newEntities = new List<PokemonEntity>();
        foreach (var pokemonResult in allPokemonList)
        {
            try
            {
                var species = await _apiService.GetPokemonSpeciesAsync(pokemonResult.Name);
                if (species == null) continue;

                var generationNumberStr = species.Generation.Name.Split('-').Last();
                var generationId = RomanToInteger(generationNumberStr.ToUpper());

                var urlSegments = pokemonResult.Url.TrimEnd('/').Split('/');
                var id = int.Parse(urlSegments.Last());

                newEntities.Add(new PokemonEntity
                {
                    Id = id,
                    Name = pokemonResult.Name,
                    GenerationId = generationId,
                    LastUpdated = DateTime.UtcNow
                });

                _logger.LogInformation("Staging for seed: #{Id} {Name} (Gen {GenId})", id, pokemonResult.Name, generationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed {Name}.", pokemonResult.Name);
            }
        }

        await _dbContext.Pokemons.AddRangeAsync(newEntities);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Database seeding complete!");
    }
    public async Task<PokemonDetail?> GetPokemonDetailsAsync(string name)
    {
        var pokemonInDb = await _dbContext.Pokemons.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());

        if (pokemonInDb != null && pokemonInDb.Height.HasValue && !string.IsNullOrEmpty(pokemonInDb.FlavorTextsJson))
        {
            return new PokemonDetail
            {
                Id = pokemonInDb.Id,
                Name = pokemonInDb.Name,
                Height = pokemonInDb.Height.Value,
                Weight = pokemonInDb.Weight ?? 0,
                Sprites = new SpriteInfo { FrontDefault = pokemonInDb.ImageUrl ?? "" },
                Types = JsonSerializer.Deserialize<List<TypeInfo>>(pokemonInDb.TypesJson ?? "[]") ?? new(),
                FlavorTexts = JsonSerializer.Deserialize<List<FlavorTextEntry>>(pokemonInDb.FlavorTextsJson ?? "[]") ?? new()
            };
        }

        var apiPokemonDetail = await _apiService.GetPokemonDetailsAsync(name);
        var apiSpeciesDetail = await _apiService.GetPokemonSpeciesAsync(name);

        if (apiPokemonDetail == null || apiSpeciesDetail == null) return null;

        // ✅ This is the corrected part. It no longer filters for ".Language.Name == "en"".
        var allFlavorTexts = apiSpeciesDetail.FlavorTextEntries
            .GroupBy(entry => $"{entry.Version.Name}-{entry.Language.Name}") // Group by game and language
            .Select(group => group.First()) // Select first entry to avoid duplicates per language/game
            .ToList();

        apiPokemonDetail.FlavorTexts = allFlavorTexts;

        var entityToUpdate = pokemonInDb ?? new PokemonEntity { Id = apiPokemonDetail.Id, Name = apiPokemonDetail.Name };

        entityToUpdate.Height = apiPokemonDetail.Height;
        entityToUpdate.Weight = apiPokemonDetail.Weight;
        entityToUpdate.ImageUrl = apiPokemonDetail.Sprites.FrontDefault;
        entityToUpdate.TypesJson = JsonSerializer.Serialize(apiPokemonDetail.Types);
        entityToUpdate.FlavorTextsJson = JsonSerializer.Serialize(allFlavorTexts); // Save all descriptions
        entityToUpdate.LastUpdated = DateTime.UtcNow;

        if (pokemonInDb == null)
        {
            _dbContext.Pokemons.Add(entityToUpdate);
        }
        else
        {
            _dbContext.Pokemons.Update(entityToUpdate);
        }
        await _dbContext.SaveChangesAsync();

        return apiPokemonDetail;
    }

    public async Task<List<EvolutionStage>?> GetEvolutionChainAsync(string pokemonName)
    {
        // We will always go to the service for the rich details, as it's memory-cached.
        // The database doesn't store the formatted trigger text.
        var apiEvolutionChain = await _apiService.GetEvolutionChainAsync(pokemonName);

        if (apiEvolutionChain == null || !apiEvolutionChain.Any())
        {
            return new List<EvolutionStage>();
        }

        
        var pokemonInDb = await _dbContext.Pokemons.FirstOrDefaultAsync(p => p.Name.ToLower() == pokemonName.ToLower());

        // Only update the DB if the chain info is missing
        if (pokemonInDb != null && string.IsNullOrEmpty(pokemonInDb.EvolutionChainIds))
        {
            var ids = apiEvolutionChain.Select(p => int.Parse(p.Pokemon.Url.TrimEnd('/').Split('/').Last())).ToList();
            var idString = string.Join(",", ids);

            var entitiesToUpdate = await _dbContext.Pokemons.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var entity in entitiesToUpdate)
            {
                entity.EvolutionChainIds = idString;
                entity.LastUpdated = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
        }

        return apiEvolutionChain;
    }

    private static int RomanToInteger(string roman)
    {
        var map = new Dictionary<char, int>
        {
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 }
        };
        int number = 0;
        for (int i = 0; i < roman.Length; i++)
        {
            if (i + 1 < roman.Length && map[roman[i]] < map[roman[i + 1]])
            {
                number -= map[roman[i]];
            }
            else
            {
                number += map[roman[i]];
            }
        }
        return number;
    }
}