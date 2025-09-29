using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;
using MediatR;

namespace CleanArch_Products.Application.Mediator.Products.Queries
{
    public class GetProductsByCategoryQuery : IRequest<IEnumerable<Product>>
    {
        public int CategoryId { get; }

        public GetProductsByCategoryQuery(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
    
}