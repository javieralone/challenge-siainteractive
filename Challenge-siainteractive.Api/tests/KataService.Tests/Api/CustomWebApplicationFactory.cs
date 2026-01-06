using Challenge.Infrastructure.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Challenge.Tests.Api;


public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = GetConfiguration();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ChallengeDBContext>));

            services.Remove(descriptor);

            services.AddDbContext<ChallengeDBContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });


            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ChallengeDBContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                db.Database.EnsureCreated();
            }
        });
    }

    private IConfigurationRoot GetConfiguration()
    {
        var integrationConfig = new ConfigurationBuilder()
               .AddJsonFile("appsettings.E2ETest.json")
               .AddEnvironmentVariables()
               .Build();

        return integrationConfig;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        var accessToken = GenerateToken();
        base.ConfigureClient(client);

        var bearerToken = "Bearer " + accessToken;
        client.DefaultRequestHeaders.Add("Authorization", bearerToken);

    }

    private string GenerateToken()
    {
        var configuration = GetConfiguration();

        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"]));

        var myIssuer = configuration["Authentication:JwtBearer:Issuer"];
        var myAudience = configuration["Authentication:JwtBearer:Audience"];

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using (var scope = Services.GetService<IServiceScopeFactory>().CreateScope())
        {
            await action(scope.ServiceProvider);
        }
    }
    public async Task ExecuteDbContextAsync(Func<ChallengeDBContext, Task> action)
    {
        await ExecuteScopeAsync(sp => action(sp.GetService<ChallengeDBContext>()));
    }

    public async Task RespawnDbContext()
    {
        await ExecuteDbContextAsync(async context =>
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
    }
}