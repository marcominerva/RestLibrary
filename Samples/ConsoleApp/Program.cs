﻿using Newtonsoft.Json;
using RestLibrary;
using RestLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new RestClient("http://localhost:27243/");

            Console.WriteLine("Trying to logging in...");

            var authenticationResult = await client.OAuthLoginAsync("demo@demo.com", "password");
            if (authenticationResult.Succeeded)
            {
                Console.WriteLine("\r\nTrying to get product list...");
                var productListResponse = await client.GetAsync<IEnumerable<Product>>("api/products");

                if (productListResponse.IsSuccessful)
                {
                    foreach (var product in productListResponse.Content)
                    {
                        PrintProductInformation(product);
                    }
                }

                Console.WriteLine("Trying to get product with ID = 96 (that doesn't exist)...");
                var productResponse = await client.GetAsync<Product>("api/products/96");
                if (productResponse.IsSuccessful)
                {
                    PrintProductInformation(productResponse.Content);
                }
                else if (productResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Product not found.");
                }

                Console.WriteLine("Adding a new product...");
                var newProduct = new Product { Name = "Mouse Bluetooth", Price = 25 };
                var newProductResponse = await client.PostWithResultAsync<Product, Product>("api/products", newProduct);

                Console.WriteLine($"The ID of the new product is {newProductResponse.Content.Id}");
            }
            else
            {
                Console.WriteLine($"{authenticationResult.Error.Message}: {authenticationResult.Error.Description}");
            }

            Console.ReadLine();
        }

        static void PrintProductInformation(Product product)
        {
            Console.WriteLine($"Product {product.Id}:");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Price: {product.Price}");
            Console.WriteLine();
        }
    }
}
