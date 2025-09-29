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
    public class ProductUpdateCommandHandler : IRequestHandler<ProductUpdateCommand, Product>
    {

        private readonly IProductRepository _productRepository;

        public ProductUpdateCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

        }

        public async Task<Product> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new ApplicationException($"Product with id {request.Id} not found");
            }

            product.Update(request.Name, request.Description, request.Price, request.Stock, request.Image, request.CategoryId);
            return await _productRepository.UpdateAsync(product);

        }
    }
}