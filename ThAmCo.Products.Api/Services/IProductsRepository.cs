using System;
namespace ThAmCo.Products.Api.Services
{
    public interface IProductsRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(string name);

        Task<ProductDto> GetProductAsync(int id);

    }
}
