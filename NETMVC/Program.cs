using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NETMVC.Data;
using System.Configuration;
using System.Security.Claims;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"));
});

// Add these lines for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/Index", () => "Hello. My secret");
app.MapGet("/api/Create", () => "Hello, World!");
app.MapGet("/api/Delte", () => "Hello, World!");
app.MapGet("/api/Edit", () => "Hello, World!");
app.MapGet("/api/Import", () => "Hello, World!");
app.MapGet("/api/ExportCSV", () => "Hello, World!");
app.MapGet("/api/ExportXLSX", () => "Hello, World!");

// Add this line to enable Swagger JSON endpoint
app.MapSwagger();

app.Run();