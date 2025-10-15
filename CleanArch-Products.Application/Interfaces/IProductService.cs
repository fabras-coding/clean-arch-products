using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.DTOs;

namespace CleanArch_Products.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
        Task<ProductDTO> GetById(int? id);
        Task<ProductDTO> GetProductAndCategory(int? id);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(int? categoryId);
        Task Add(CreateProductDTO product);
        Task Update(ProductDTO product);
        Task Remove(int? id);

    }
}