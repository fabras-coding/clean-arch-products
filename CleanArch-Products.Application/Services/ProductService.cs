using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;

namespace CleanArch_Products.Application.Services
{
    public class ProductService : IProductService
    {

        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper;
        }

        public async Task Add(ProductDTO product)
        {
            var productEntity = _mapper.Map<Product>(product);  
            await _productRepository.CreateAsync(productEntity);
        }

        public async Task<ProductDTO> GetById(int? id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> GetProductAndCategory(int? id)
        {
            var product = await _productRepository.GetProductAndCategoryAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var products= await _productRepository.GetProductsAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(int? categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task Remove(int? id)
        {
            var product = _productRepository.GetByIdAsync(id).Result;
             await _productRepository.RemoveAsync(product);
        }

        public async Task Update(ProductDTO product)
        {
            var productEntity = _mapper.Map<Product>(product);
            await _productRepository.UpdateAsync(productEntity);
        }
    }
}