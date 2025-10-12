using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArch_Products.Application.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "The Name field is required")]
        [StringLength(100, ErrorMessage = "The maximum length is 100 characters")]
        [MinLength(3, ErrorMessage = "The minimum length is 3 characters")]
        [DisplayName("Category Name")]

        public string Name { get; set; }
    }
}