using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Domain.Interfaces;
using CleanArch_Products.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch_Products.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register your infrastructure services here
            // e.g., database context, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(Data.Context.ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IProductRepository, Data.Repositories.ProductRepository>();
            services.AddScoped<ICategoryRepository, Data.Repositories.CategoryRepository>();

            return services;
        }
    }
}