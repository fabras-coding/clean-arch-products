using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Application.Mappings;
using CleanArch_Products.Application.Messaging;
using CleanArch_Products.Application.Services;
using CleanArch_Products.Domain.Interfaces;
using CleanArch_Products.Infra.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace CleanArch_Products.Infra.IoC
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            // Register your infrastructure services here
            // e.g., database context, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(Data.Context.ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IProductRepository, Data.Repositories.ProductRepository>();
            services.AddScoped<ICategoryRepository, Data.Repositories.CategoryRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddSingleton<IMessageBus>(provider=>
            {

                var messageBusProvider = configuration.GetValue<string>("MessageBus:Provider");

                return messageBusProvider switch
                {
                    "Kafka" => new Utils.Messaging.KafkaMessageBus(configuration.GetValue<string>("Kafka:BootstrapServers")),
                    "SQS" => new Utils.Messaging.SQSMessageBus(
                        configuration.GetValue<string>("AWS.SQS:ServiceURL"),
                        configuration.GetValue<string>("AWS.SQS:QueueName"),
                        configuration.GetValue<string>("AWS.SQS:Region"),
                        configuration.GetValue<string>("AWS.SQS:AccessKey"),
                        configuration.GetValue<string>("AWS.SQS:SecretKey")),

                    _ => throw new Exception("Invalid message bus provider configuration. Check appsettings.json"),


                };
                
                
            });

            var myHandlers = AppDomain.CurrentDomain.Load("CleanArch-Products.Application");
            services.AddMediatR(myHandlers);

            return services;
        }
    }
}