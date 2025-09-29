using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Application.Mediator.Products.Commands;
using CleanArch_Products.Application.Mediator.Products.Queries;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using MediatR;

namespace CleanArch_Products.Application.Services
{
    public class ProductService : IProductService
    {

        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ProductService(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        
        }

        public async Task Add(ProductDTO product)
        {
            var productCommand = _mapper.Map<ProductCreateCommand>(product);
            await _mediator.Send(productCommand); 
            
        }

        public async Task<ProductDTO> GetById(int? id)
        {
            var productQuery = new GetProductByIdQuery(id.Value);

            if (productQuery == null)
            {
                throw new ApplicationException("Entity could not be loaded.");
            }
            var product = await _mediator.Send(productQuery);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> GetProductAndCategory(int? id)
        {
            var productQuery = new GetProductAndCategory(id.Value);
            if (productQuery == null)
            {
                throw new ApplicationException("Entity could not be loaded.");
            }
            var product = await _mediator.Send(productQuery);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var productsQuery = new GetProductsQuery();

            if (productsQuery == null)
            {
                throw new ApplicationException("Entity could not be loaded.");
            }

            var products = await _mediator.Send(productsQuery);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);

        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(int? categoryId)
        {
            var productsQuery = new GetProductsByCategoryQuery(categoryId.Value);
            if (productsQuery == null)
            {
                throw new ApplicationException("Entity could not be loaded.");
            }
            var products = await _mediator.Send(productsQuery);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
            
            
        }

        public async Task Remove(int? id)
        {
            var productCommand = new ProductRemoveCommand(id.Value);
            if (productCommand == null)
                throw new ApplicationException("Entity could not be loaded.");

            await _mediator.Send(productCommand);
        }

        public async Task Update(ProductDTO product)
        {

            var productCommand = _mapper.Map<ProductUpdateCommand>(product);
            await _mediator.Send(productCommand);
            
        }
    }
}