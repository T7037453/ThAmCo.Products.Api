using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using System.Configuration;
using System.Text.Json;
using System.Transactions;
using ThAmCo.Products.Api.Data;
using ThAmCo.Products.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "products.db");
        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();

    }
    else
    {
        var ps = builder.Configuration.GetConnectionString("ProductsContext");
        options.UseSqlServer(ps);
    }
});

builder.Services
     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    options.Authority = builder.Configuration["Auth:Authority"];
    options.Audience = builder.Configuration["Auth:Audience"];
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadAccess", policy =>
                      policy.RequireAssertion(context =>
                      context.User.HasClaim(claim =>
                      (claim.Type == "permissions" &&
                      (claim.Value == "read:details") &&
                      claim.Issuer == $"https://{builder.Configuration["Auth:Domain"]}/"
                      )
                      )
                      ));
});


//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();

app.UseAuthorization();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    //app.UseSwagger();
//    //app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

var responseMessage = app.Configuration["Message"] ?? "";

app.MapPost("/products", async (ProductsContext ctx, ProductDto dto) =>
{
    var product = new Product { Id = dto.Id, Name = dto.Name, Brand = dto.Brand, Description = dto.Description, Price = dto.Price, StockLevel = dto.StockLevel };
    await ctx.AddAsync(product);
    await ctx.SaveChangesAsync();
    return responseMessage;
});


app.MapGet("/products", async (ProductsContext ctx) =>
{
    return await ctx.Products.ToListAsync();
});

app.MapGet("/products/{id}", [Authorize(Policy ="ReadAccess")] async (ProductsContext ctx, int id) =>
{
    return await ctx.Products.FindAsync(id);
});;

app.MapDelete("/products/{id}", [Authorize(Policy = "ReadAccess")] async (ProductsContext ctx, int id) =>
{
    var product = await ctx.Products.FindAsync(id);
    ctx.Products.Remove(product);
    ctx.SaveChanges();
    return responseMessage;
});

app.MapPut("/products/{id}", [Authorize(Policy = "ReadAccess")] async (ProductsContext ctx, Product product, int id) =>
{
    var productToUpdate = await ctx.Products.FindAsync(id);
    productToUpdate.Name = product.Name;
    productToUpdate.Brand = product.Brand;
    productToUpdate.Description = product.Description;
    productToUpdate.Price = product.Price;
    productToUpdate.StockLevel = product.StockLevel;

    await ctx.SaveChangesAsync();

    return Results.Ok(productToUpdate);

});




//app.UseAuthorization();

//app.MapControllers();

app.Run();

record ProductDto (int Id, string Name, string Brand, string Description, double Price, int StockLevel);
