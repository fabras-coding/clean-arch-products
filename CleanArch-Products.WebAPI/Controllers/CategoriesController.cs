using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch_Products.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            try
            {
                var categories = await _categoryService.GetCategories();
                if (categories == null)
                    return NotFound("Categories not found");

                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                //logger can be added here to log the exception details
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                if (category == null)
                    return NotFound("Category not found");

                return Ok(category);
            }
            catch (System.Exception ex)
            {
                //logger can be added here to log the exception details
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateCategoryDTO categoryDTO)
        {
            try
            {
                if (categoryDTO == null)
                    return BadRequest("Invalid data.");

                await _categoryService.Add(categoryDTO);

                return Ok(categoryDTO);
            }
            catch (System.Exception ex)
            {
                //logger can be added here to log the exception details
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                if (categoryDTO == null)
                    return BadRequest("Invalid data.");

                await _categoryService.Update(categoryDTO);

                return Ok(categoryDTO);
            }
            catch (System.Exception ex)
            {
                //logger can be added here to log the exception details
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                if (category == null)
                    return NotFound("Category not found.");

                await _categoryService.Remove(id);

                return Ok("Category removed successfully.");
            }
            catch (System.Exception ex)
            {
                //logger can be added here to log the exception details
                return StatusCode(500, "Internal server error");

            }
        }

    }
}