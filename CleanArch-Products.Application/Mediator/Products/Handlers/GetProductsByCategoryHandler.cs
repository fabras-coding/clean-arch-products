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
    public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<Product>>
    {
        
        IProductRepository _productRepository;
        public GetProductsByCategoryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            
        }
        public async Task<IEnumerable<Product>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsByCategoryIdAsync(request.CategoryId);

        }
    }
}