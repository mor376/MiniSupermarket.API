using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniSupermarket.API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DATABASE CONFIGURATION
// =========================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SupermarketDbContext>(options =>
    options.UseSqlServer(connectionString));


// =========================
// JWT CONFIGURATION
// =========================

var jwtSecret =
    builder.Configuration["JwtSettings:Secret"]
    ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtSecret)
                ),

            ValidateIssuer = false,
            ValidateAudience = false
        };
});


// =========================
// CONTROLLERS
// =========================

builder.Services.AddControllers(options =>
{
    options.Filters.Add<
        MiniSupermarket.API.Filters.RequestTimingFilter>();
});


// =========================
// SWAGGER
// =========================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",

            Name = "Authorization",

            In =
                Microsoft.OpenApi.Models.ParameterLocation.Header,

            Type =
                Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,

            Scheme = "Bearer"
        });

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                new string[] { }
            }
        });
});


// =========================
// BUILD APPLICATION
// =========================

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SupermarketDbContext>();
    dbContext.Database.Migrate();
    dbContext.EnsureProductStatusColumnExists();
    dbContext.EnsureProductCodesAreNumeric();
    dbContext.EnsureCustomerStatusColumnExists();
    dbContext.EnsureDemoUsersSeeded();
    dbContext.EnsureDemoCustomersSeeded();
    dbContext.EnsureSuppliersTableExists();
    dbContext.EnsureDemoSuppliersSeeded();
    app.Logger.LogInformation("Database migrations applied successfully and demo data verified.");
}
catch (Exception ex)
{
    app.Logger.LogWarning(
        ex,
        "Database migration failed. Ensure SQL Server / LocalDB is running before using the API. The app will continue to start in degraded mode."
    );
}

// =========================
// SWAGGER
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// =========================
// HTTP PIPELINE
// =========================

app.UseHttpsRedirection();


// Authentication must come before Authorization
app.UseAuthentication();

app.UseAuthorization();


// =========================
// CONTROLLERS
// =========================

app.MapControllers();

app.Run();