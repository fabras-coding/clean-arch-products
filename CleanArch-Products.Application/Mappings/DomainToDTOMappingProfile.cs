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

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();

            
        }

        
    }
}