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
    public class GetProductsByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
    {

        IProductRepository _productRepository;  

        public GetProductsByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }
        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetByIdAsync(request.Id);
        }
    }

   
}