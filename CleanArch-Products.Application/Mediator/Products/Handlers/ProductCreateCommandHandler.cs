using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.Mediator.Products.Commands;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using MediatR;

namespace CleanArch_Products.Application.Mediator.Products.Handlers
{
    public class ProductCreateCommandHandler : IRequestHandler<ProductCreateCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        public ProductCreateCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.Description, request.Price, request.Stock, request.Image, request.CategoryId);

            if (product == null)
            {
                throw new ApplicationException("Error creating product");
            }   
            product.CategoryId = request.CategoryId;
            return await _productRepository.CreateAsync(product);
            
        }
        
    }
}