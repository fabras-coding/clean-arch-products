using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;

namespace CleanArch_Products.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {

            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task Add(CategoryDTO category)
        {
            var categoryEntity = _mapper.Map<Domain.Entities.Category>(category);
            await _categoryRepository.Create(categoryEntity);
        }

        public async Task<CategoryDTO> GetById(int? id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task Remove(int? id)
        {
            var category = _categoryRepository.GetByIdAsync(id).Result;
            await _categoryRepository.Remove(category);
        }

        public async Task Update(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.Update(category);
        }
    }
}