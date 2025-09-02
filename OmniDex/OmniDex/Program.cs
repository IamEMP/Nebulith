
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OmniDex.Components;
using OmniDex.Data;
using OmniDex.Repositories;
using OmniDex.Services;
using OmniDex.State;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<PokeApiService>();
builder.Services.AddHttpClient<PokeApiService>();
builder.Services.AddMudServices();
builder.Services.AddDbContext<PokedexDbContext>(options =>
    options.UseSqlite("Data Source=pokedex.db"));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<PokemonRepository>();
builder.Services.AddScoped<PokedexStateService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var repository = scope.ServiceProvider.GetRequiredService<PokemonRepository>();
        // ? Await the task to ensure it completes before the app is running
        await repository.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        // Log any errors that occur during seeding
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    
    


app.Run();