using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using VoltHub.Api.ErrorHandling;
using VoltHub.Api.OpenApi;
using VoltHub.Api.Services;
using VoltHub.Application;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Infrastructure;
using VoltHub.Infrastructure.Authentication;
using VoltHub.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. The layers: use cases (Application) and database/security implementations (Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Web pieces: controllers, the current user, RFC 7807 error responses
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 3. Authentication: every request's JWT is checked for signature, issuer, audience and expiry
var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
if (jwt is null || string.IsNullOrWhiteSpace(jwt.SigningKey) || jwt.SigningKey.Length < 32)
    throw new InvalidOperationException("Jwt:SigningKey is missing or shorter than 32 characters. Set it with dotnet user-secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;   // keep the token's claim names ("sub", "role") as they are
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub",
            RoleClaimType = "role"          // makes [Authorize(Roles = "Admin")] work
        };
    });
builder.Services.AddAuthorization();

// 4. Rate limiting: at most 10 login/register/refresh requests per minute per IP address
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

// 5. OpenAPI document (+ JWT scheme) for the Scalar UI
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();   // https://localhost:<port>/scalar
    await DataSeeder.SeedAsync(app.Services, app.Configuration);
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
