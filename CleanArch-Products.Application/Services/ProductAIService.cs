using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Domain.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

namespace CleanArch_Products.Application.Services
{
    public class ProductAIService
    {
        private readonly IConfiguration _configuration;
        

        public ProductAIService(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }

        public async Task<string> AskProductQuestion(ProductDTO product, string question)
        {

            var apiKey = _configuration["OpenAI_Key"] ?? throw new InvalidOperationException("OpenAI API key is not configured.");
            var _openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
            {
                Endpoint = new Uri("https://models.inference.ai.azure.com") // Azure OpenAI endpoint
            });

            IChatClient client = _openAIClient.GetChatClient("gpt-4o").AsIChatClient();

            var prompt = $"""
                    You are an assistant that provides information about products based ONLY on the following information:
                    {product.InformationDocument}
                    Customer question: {question}   

                    If the question is not related to the product information, please respond with "I'm sorry, I can only answer questions related to the product information provided."

                 """;

            var response = await client.GetResponseAsync(prompt);
            return response.Messages.FirstOrDefault()?.Text ?? "No response from AI.";

        }

    }
}