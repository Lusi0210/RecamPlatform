using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.API.Mapping;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Repository;
using Remp.Remp.Service;
using Remp.Remp.API.Middlewares;
using Microsoft.AspNetCore.Identity;
using Remp.Remp.API.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;  


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("Remp.API/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Remp.API/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<RempDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<RempDbContext>()
    .AddDefaultTokenProviders();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<IListingCaseRepository, ListingCaseRepository>();
builder.Services.AddScoped<IListingCaseService, ListingCaseService>();
builder.Services.AddAutoMapper(cfg => {}, typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<CreateListingCaseRequestDto>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateListingCaseRequestDto>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
builder.Services.AddScoped<IMediaAssetService, MediaAssetService>();

builder.Services.AddScoped<ICaseContactRepository, CaseContactRepository>();
builder.Services.AddScoped<ICaseContactService, CaseContactService>();

builder.Services.AddScoped<IAgentPhotographyCompanyRepository, AgentPhotographyCompanyRepository>();
builder.Services.AddScoped<IAgentPhotographyCompanyService, AgentPhotographyCompanyService>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

using (IServiceScope scope = app.Services.CreateScope())
{
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    RempDbContext dbContext = scope.ServiceProvider.GetRequiredService<RempDbContext>();
    await RoleSeeder.SeedAdminAsync(userManager, dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.MapControllers();
app.Run();