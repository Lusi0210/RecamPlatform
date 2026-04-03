using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.API.Mapping;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Repository;
using Remp.Remp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<RempDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IListingCaseRepository, ListingCaseRepository>();
builder.Services.AddScoped<IListingCaseService, ListingCaseService>();
builder.Services.AddAutoMapper(cfg => {}, typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<CreateListingCaseRequestDto>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateListingCaseRequestDto>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();