using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using CleanArch_Products.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch_Products.Infra.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        ApplicationDbContext _categoryRepository;
        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            _categoryRepository = applicationDbContext; ;
        }

        public async Task<Category> Create(Category category)
        {
            _categoryRepository.Add(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Category> GetByIdAsync(int? id)
        {
            return _categoryRepository.Categories.FindAsync(id).Result;
            
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return _categoryRepository.Categories.ToListAsync().Result;
        }

        public async Task<Category> Remove(Category category)
        {
            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Category> Update(Category category)
        {
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }
    }
}