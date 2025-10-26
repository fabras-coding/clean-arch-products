using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Validation;

namespace CleanArch_Products.Domain.Entities
{
    public sealed class Category : EntityBase
    {

        public string Name { get; private set; }


        public Category(string name)
        {
            ValidateDomain(name);

        }

         public Category(int id, string name)
        {
            DomainExceptionValidation.When(id <= 0, "invalid Id value. Must be greater than zero");
                
            ValidateDomain(name);
            Id = id;
        }

        public void Update(string name)
        {
            ValidateDomain(name);
        }

        private void ValidateDomain(string name)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name), "Invalid name. Name is required.");
            DomainExceptionValidation.When(name.Length < 3, "Invalid name, minimum 3 characters.");
            Name = name;

        }

       
        
        

        
    }
}