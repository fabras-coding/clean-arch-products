using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Domain.Entities;

namespace CleanArch_Products.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<CreateCategoryDTO, Category>().ConstructUsing(c => new Category(c.Name));
            CreateMap<CreateProductDTO, Product>().ConstructUsing(p => new Product(p.Name, p.Description, p.Price, p.Stock, p.Image, p.CategoryId));

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();

            
        }

        
    }
}