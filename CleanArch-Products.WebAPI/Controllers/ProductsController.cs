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
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = await _productService.GetProducts();
            if (products == null || !products.Any())
                return NotFound();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategoryId(int id)
        {
            var products = await _productService.GetProductsByCategoryId(id);
            if (products == null)
                return NotFound();

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                await _productService.Add(productDTO);
                return Ok();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO productDTO)
        {
            if (id != productDTO.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _productService.Update(productDTO);
                return Ok();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
                return NotFound();

            await _productService.Remove(id);
            return Ok();
        }

    }
}