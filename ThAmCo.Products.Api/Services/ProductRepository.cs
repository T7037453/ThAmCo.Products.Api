﻿using System;
using System.Net;

namespace ThAmCo.Products.Api.Services
{
    public class ProductRepository : IProductsRepository
    {
        private readonly HttpClient _client;
        public ProductRepository(HttpClient client,
                            IConfiguration configuration)
        {
            var baseUrl = configuration["WebServices:Products:BaseURL"];
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }
        public async Task<ProductDto> GetProductAsync(int id)
        {
            var response = await _client.GetAsync("/products?id=" + id);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;

            }
            //response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAsAsync<ProductDto>();
            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string name)
        {
            var uri = "/products?description=Test_Desc";
            if (name != null)
            {
                uri = uri + "&name=" + name;

            }
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDto>>();
            return products;
        }
    }
}
