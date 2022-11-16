using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MinimalAPI_ADO_Client;

class Program
{
    static HttpClient client = new HttpClient();

    static void Main()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("https://localhost:7152/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            // Fetch all existing category records.
            var categories = await GetAllCategories();
            foreach(var cate in categories)
            {
                ShowCategory(cate);
            }

            // Create a new Category
            Category category = new Category
            {
                Categoryid = 0,
                CategoryName = "Demo 1",
                Description = "Demo 1"
            };

            var url = await CreateCategoryAsync(category);
            Console.WriteLine($"Created at {url}");

            // Get the Category
            category = await GetCategoryAsync(url.ToString());
            ShowCategory(category);

            Console.WriteLine("Category record created. Please check in DB...");
            Console.WriteLine("Press <ENTER> to Update this record...");
            Console.ReadLine();

            // Update the Category
            Console.WriteLine("Updating description...");
            category.Description = "Demo 1 changed.";
            await UpdateCategoryAsync(category);

            // Get the updated Category
            category = await GetCategoryAsync(url.ToString());
            ShowCategory(category);

            Console.WriteLine("Category record created. Please check in DB...");
            Console.WriteLine("Press <ENTER> to Delete this record...");
            Console.ReadLine();

            // Delete the Category
            var statusCode = await DeleteCategoryAsync(category.Categoryid);
            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");
            Console.WriteLine("Press <ENTER> to exit...");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadLine();
    }

    // Install-Package Microsoft.AspNet.WebApi.Client
    static void ShowCategory(Category category)
    {
        Console.WriteLine($"ID: {category.Categoryid}\t Name: {category.CategoryName}\t" +
            $"Description: {category.Description}");
    }

    static async Task<List<Category>> GetAllCategories()
    {
        List<Category> categories = new List<Category>();
        var path = "categories";
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            categories = await response.Content.ReadAsAsync<List<Category>>();
        }
        return categories;
    }

    static async Task<Uri> CreateCategoryAsync(Category category)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "categories", category);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }

    static async Task<Category> GetCategoryAsync(string path)
    {
        Category category = null;
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            category = await response.Content.ReadAsAsync<Category>();
        }
        return category;
    }

    static async Task<Category> UpdateCategoryAsync(Category category)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"categories/{category.Categoryid}", category);
        response.EnsureSuccessStatusCode();

        // Deserialize the updated Category from the response body.
        category = await response.Content.ReadAsAsync<Category>();
        return category;
    }

    static async Task<HttpStatusCode> DeleteCategoryAsync(long id)
    {
        HttpResponseMessage response = await client.DeleteAsync(
            $"categories/{id}");
        return response.StatusCode;
    }

}
