using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;

namespace CleanArch_Products.Domain.Interfaces
{
    public interface IProductRepository
    {
        
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetByIdAsync(int? id);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int? id);

        Task<Product> Create(Product category);
        Task<Product> Update(Product category);
        Task<Product> Remove(Product category);
    }
}