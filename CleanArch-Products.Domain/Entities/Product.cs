using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Validation;

namespace CleanArch_Products.Domain.Entities
{
    public class Product : EntityBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Image { get; private set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public Product(string name, string description, decimal price, int stock, string image)
        {
            ValidateDomain(name, description, price, stock, image);
        }

        public Product(int id, string name, string description, decimal price, int stock, string image)
        {
            DomainExceptionValidation.When(id <= 0, "Invalid id value. Must be greater than zero");
            Id = id;
            ValidateDomain(name, description, price, stock, image);
        }

        public void Update(string name, string description, decimal price, int stock, string image, int categoryId)
        {
            ValidateDomain(name, description, price, stock, image);
            DomainExceptionValidation.When(categoryId <= 0, "Invalid id value. Must be greater than zero");
            CategoryId = categoryId;

        }

        private void ValidateDomain(string name, string description, decimal price, int stock, string image)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name), "Invalid name. Name is required.");
            DomainExceptionValidation.When(name.Length < 3, "Invalid name, too short. The minimum is 3 characters.");
            DomainExceptionValidation.When(string.IsNullOrEmpty(description), "invalid description. Description is required.");
            DomainExceptionValidation.When(description.Length < 10, "Invalid description, too short. The minimum is 10 characters.");
            DomainExceptionValidation.When(price < 0, "Invalid value. Price must be greater than zero.");
            DomainExceptionValidation.When(stock < 0, "Invalid value. Stock must be greater than zero.");
            DomainExceptionValidation.When(image?.Length > 250, "Invalid image name, too long. The maximum is 250 characters.");

            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Image = image;


        }


    }
}