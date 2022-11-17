using Microsoft.EntityFrameworkCore;
using System.Transactions;
using ThAmCo.Products.Api.Data;

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

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.MapGet("/products/details", async (ProductsContext ctx, int id) =>
{
    return await ctx.Products.FindAsync(id);
});



//app.UseAuthorization();

//app.MapControllers();

app.Run();

record ProductDto (int Id, string Name, string Brand, string Description, double Price, int StockLevel);
