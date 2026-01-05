using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Challenge.Infrastructure.CrossCutting.Authentications;

public static class AuthenticationExtension
{
    public static void AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    if (context.AuthenticateFailure == null)
                        throw new Exception("Authentication token is not present");
                    else
                        throw new Exception("Authentication token validation failed: " + context.AuthenticateFailure.Message);
                }
            };

            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
        });
    }
}        
