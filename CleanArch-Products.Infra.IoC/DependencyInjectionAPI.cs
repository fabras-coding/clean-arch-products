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
using StackExchange.Redis;
using CleanArch_Products.Application.Idempotency;
using CleanArch_Products.Infra.Utils.Idempotency;
using Microsoft.Extensions.Logging;



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

            services.AddTransient<IProductRepository, Data.Repositories.ProductRepository>();
            services.AddTransient<ICategoryRepository, Data.Repositories.CategoryRepository>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<ProductAIService>();
            
            services.AddSingleton<IMessageBus>(provider=>
            {

                var messageBusProvider = configuration.GetValue<string>("MessageBus:Provider");
                var logger = provider.GetRequiredService<ILogger<Utils.Messaging.KafkaMessageBus>>();

                return messageBusProvider switch
                {
                    "Kafka" => new Utils.Messaging.KafkaMessageBus(configuration.GetValue<string>("Kafka:BootstrapServers") ?? throw new InvalidOperationException("Kafka bootstrap servers configuration is not configured."), logger),
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


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("redis") ?? throw new InvalidOperationException("Redis connection string is not configured.");
                options.InstanceName = "CleanArchProductsCache_";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString("redis") ?? throw new InvalidOperationException("Redis connection string is not configured.")));
            services.AddSingleton<IIdempotencyStore, RedisIdempotencyStore>();

            return services;
        }
    }
}