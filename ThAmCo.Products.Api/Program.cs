using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "prducts.db");
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

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var responseMessage = app.Configuration["Message"] ?? "";

app.MapPost("/products", async (ProductsContext ctx, ProductDto dto) =>
{
    var product = new Product { Id = dto.Id, Name = dto.Name };
    await ctx.AddAsync(product);
    await ctx.SaveChangesAsync();
    return responseMessage;
});

app.MapGet("/products", async (ProductsContext ctx) =>
{
    return await ctx.Products.ToListAsync();
});

app.UseAuthorization();

app.MapControllers();

app.Run();

record ProductDto (int Id, string Name);
