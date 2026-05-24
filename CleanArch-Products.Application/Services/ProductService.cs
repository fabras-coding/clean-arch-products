using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Application.Mediator.Products.Commands;
using CleanArch_Products.Application.Mediator.Products.Queries;
using CleanArch_Products.Application.Messaging;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArch_Products.Application.Services
{
    public class ProductService : IProductService
    {

        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMessageBus _messageBus;
        private readonly IDistributedCache _cache;

        private const string ALL_PRODUCTS_CACHE_KEY = "products:all";
        private static readonly TimeSpan CacheTTL = TimeSpan.FromSeconds(60);

        public ProductService(IMapper mapper, IMediator mediator, IMessageBus messageBus, IDistributedCache cache)
        {
            _mapper = mapper;
            _mediator = mediator;
            _messageBus = messageBus;
            _cache = cache;
            
        
        }

        public async Task Add(CreateProductDTO product)
        {
            var productCommand = _mapper.Map<ProductCreateCommand>(product);
            await _mediator.Send(productCommand); 
            await _messageBus.PublishAsync("product-created", product);
            
            await _cache.RemoveAsync(ALL_PRODUCTS_CACHE_KEY);
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
            var cachedProducts = await _cache.GetStringAsync(ALL_PRODUCTS_CACHE_KEY);
            if(cachedProducts is not null)
                return JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(cachedProducts);
            
            
            var productsQuery = new GetProductsQuery();

            if (productsQuery == null)
            {
                throw new ApplicationException("Entity could not be loaded.");
            }

            var products = await _mediator.Send(productsQuery);
            var dto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            _ = _cache.SetStringAsync(ALL_PRODUCTS_CACHE_KEY, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheTTL
            });

            return dto;

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
            await _cache.RemoveAsync(ALL_PRODUCTS_CACHE_KEY);
        }

        public async Task Update(ProductDTO product)
        {

            var productCommand = _mapper.Map<ProductUpdateCommand>(product);
            await _mediator.Send(productCommand);
            await _cache.RemoveAsync(ALL_PRODUCTS_CACHE_KEY);
            
        }
    }
}