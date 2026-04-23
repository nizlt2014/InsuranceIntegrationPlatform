using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PolicyApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ServiceBusPublisher>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Policy API",
        Version = "v1"
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/a87e1acd-163d-4fef-9fe1-9c4776a1b723/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/a87e1acd-163d-4fef-9fe1-9c4776a1b723/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    {
                        "api://86e7e2a7-4439-4228-ac7c-1d8d5fd8866f/access_as_user",
                        "Access Policy API"
                    }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[]
            {
                "api://86e7e2a7-4439-4228-ac7c-1d8d5fd8866f/access_as_user"
            }
        }
    });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.OAuthClientId("138ab067-9814-4a82-a1f4-f4732973ccbd");
    options.OAuthAppName("Swagger Policy API");
    options.OAuthUsePkce();
    options.OAuthScopeSeparator(" ");
    options.OAuthScopes("api://86e7e2a7-4439-4228-ac7c-1d8d5fd8866f/access_as_user");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();