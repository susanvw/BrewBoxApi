using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BrewBoxApi.Infrastructure;
using BrewBoxApi.Infrastructure.Interceptors;
using BrewBoxApi.Infrastructure.Repositories;
using BrewBoxApi.Presentation.Filters;
using Microsoft.AspNetCore.Identity;
using BrewBoxApi.Infrastructure.Identity;
using BrewBoxApi.Presentation.Services;
using BrewBoxApi.Presentation.Features.Auth;
using BrewBoxApi.Presentation.Features.Orders;
using BrewBoxApi.Domain.Aggregates.Orders;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework Core with Azure SQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(new AuditInterceptor(builder.Services.BuildServiceProvider().GetRequiredService<ICurrentUserService>())));

// Configure ASP.NET Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });
});

// Configure Authorization
builder.Services.AddAuthorization();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:5173");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Authorization, Content-Type");
                context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Services and Repositories
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthControllerImplementation, AuthControllerImplementation>();
builder.Services.AddScoped<IOrderControllerImplementation, OrderControllerImplementation>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Add Exception Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<NotFoundExceptionFilter>();
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BrewBox API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure HTTPS port
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5196, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();