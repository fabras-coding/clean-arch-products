using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.Mediator.Products.Queries;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using MediatR;

namespace CleanArch_Products.Application.Mediator.Products.Handlers
{
    public class GetProductAndCategoryHandler : IRequestHandler<GetProductAndCategory, Product>
    {
        private readonly IProductRepository _productRepository;
        public GetProductAndCategoryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Product> Handle(GetProductAndCategory request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductAndCategoryAsync(request.Id);
        }
    }
}