using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Mediator.Products.Commands;

namespace CleanArch_Products.Application.Mappings
{
    public class DTOTOCommandMappingProfile : Profile
    {
        public DTOTOCommandMappingProfile()
        {
            CreateMap<CreateProductDTO, ProductCreateCommand>();
            CreateMap<ProductDTO, ProductUpdateCommand>();

        }
    }
}