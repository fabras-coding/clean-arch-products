using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Entities;

namespace CleanArch_Products.Application.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The Name field is required")]
        [StringLength(100, ErrorMessage = "The maximum length is 100 characters")]
        [MinLength(3, ErrorMessage = "The minimum length is 3 characters")]
        [DisplayName("Name")]

        public string Name { get;  set; }
        [Required(ErrorMessage = "The Description field is required")]
        [StringLength(255, ErrorMessage = "The maximum length is 255 characters")]
        [MinLength(10, ErrorMessage = "The minimum length is 10 characters")]
        [DisplayName("Description")]
        public string Description { get;  set; }

        [Required(ErrorMessage = "The Price field is required")]
        public decimal Price { get;  set; }
        public int Stock { get;  set; }
        public string Image { get;  set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}