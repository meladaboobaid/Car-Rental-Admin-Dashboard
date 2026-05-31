using Car_Rental_Dashboard_Server.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container...
var secretKey = builder.Configuration["JWT_SECRET"];
var ValidIssuer = builder.Configuration["JWT_ISSUER"];
var ValidAudience  = builder.Configuration["JWT_AUDIENCE"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new Exception("JWT secret key is not configured in environment variables.");
}
if (string.IsNullOrWhiteSpace(ValidIssuer))
{
    throw new Exception("JWT issuer is not configured in environment variables.");
}
if (string.IsNullOrWhiteSpace(ValidAudience))
{
    throw new Exception("JWT audience is not configured in environment variables.");
}

// Read connection string and assign it to your Data Access Layer class
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Database connection string is not configured in environment variables or appsettings.");
}
Car_Rental_Dashboard_Data_Access_Layer.DataAccessSettings.connectionString = connectionString;

// Authentication configuration settings
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validates claims ...
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = ValidIssuer,
        ValidAudience = ValidAudience,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("UserOwnerOrSuperAdmin", policy =>
    policy.Requirements.Add(new UserOwnerOrSuperAdminRequirement()));

});
builder.Services.AddSingleton<IAuthorizationHandler, UserOwnerOrSuperAdminHandler>();

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("UsersLimiter", httpContext =>
    {
        // 1. Try to get the authenticated user ID from the JWT claims 
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // 2. If the user is authenticated, use their ID. Otherwise, fallback to the IP Address.
        var partitionKey = !string.IsNullOrEmpty(userId)
            ? userId
            : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 150, 
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("GlobalAnonymousLimiter", httpContext =>
    {

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 150,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0 // Or set a small queue (e.g., 10) to smooth out bursts
            });
    });
});
//

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define the "Bearer" security scheme.
    // Swagger will use it to display an input box for Authorization header.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // The header name where the JWT should be placed.
        Name = "Authorization",

        // HTTP authentication scheme (Authorization header).
        Type = SecuritySchemeType.Http,

        // The scheme name must be "Bearer" for JWT Bearer tokens.
        Scheme = "Bearer",

        // Optional, but helps documentation and UI.
        BearerFormat = "JWT",

        // Token is sent in the request header.
        In = ParameterLocation.Header,

        // Instruction shown in Swagger UI.
        Description = "Enter: Bearer {your JWT token}"
    });

    // Apply the Bearer scheme globally so secured endpoints in Swagger
    // can automatically include the Authorization header after you authorize.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                // Reference the security scheme defined above by its Id: "Bearer".
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            // No specific scopes are required for JWT Bearer in this setup.
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CarRentalsApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
            "https://localhost:7035",
            "https://localhost:5215"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CarRentalsApiCorsPolicy");

// OPTIONAL (Only if behind proxy, enable this BEFORE rate limiting)
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
    // ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

app.UseAuthentication();
app.UseAuthorization();

// Global 403 logging middleware (place it HERE)
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.ToString();


        // ✅ Centralized security log for authorization abuse
        app.Logger.LogWarning(
            "Forbidden access. UserId={UserId}, Path={Path}, IP={IP}",
            userId,
            path,
            ip
        );
    }
});
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        await context.Response.WriteAsync("Too many login attempts. Please try again later.");
    }
});

app.UseRateLimiter();

app.MapControllers();
app.Run();
