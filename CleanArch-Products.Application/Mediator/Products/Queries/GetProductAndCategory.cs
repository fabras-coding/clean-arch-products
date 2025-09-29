using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;
using MediatR;

namespace CleanArch_Products.Application.Mediator.Products.Queries
{
    public class GetProductAndCategory : IRequest<Product>
    {
        
        public int Id { get; set; }
        public GetProductAndCategory(int id)
        {
            id = Id;
        }
    }
}