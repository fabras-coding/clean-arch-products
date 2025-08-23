using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using CleanArch_Products.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch_Products.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {

        ApplicationDbContext _productRepository;
        public ProductRepository(ApplicationDbContext applicationDbContext)
        {
            _productRepository = applicationDbContext; ;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetByIdAsync(int? id)
        {
            return _productRepository.Products.FindAsync(id).Result;
        }

        public Task<Product> GetProductAndCategoryAsync(int? id)
        {
            //Applying eager loading to include the related Category entity
            return _productRepository.Products.Include(c => c.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return _productRepository.Products.ToListAsync().Result;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int? id)
        {
            return _productRepository.Products.Where(p => p.CategoryId == id).ToListAsync().Result;
        }

        public async Task<Product> RemoveAsync(Product product)
        {
            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }
    }
}