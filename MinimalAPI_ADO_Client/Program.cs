using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RSTS.Userzone;

// Running the following command in NuGet package manager console is
// necessary to run this project:
/// Install-Package Microsoft.AspNet.WebApi.Client
// Alternatively, navigate NuGet to find the package.


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
        client.BaseAddress = new Uri("https://localhost:7236/");//("https://localhost:7152/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
/*            // All tickets
            var tickets = await GetAllTickets();
            foreach (var t in tickets)
            {
                ShowTicket(t);
            }

            Ticket icecream = new Ticket(2, 99, "ice cream");
            Console.WriteLine("await ice cream");
            var urlice = await CreateTicketAsync(icecream);
            Console.WriteLine($"Created at {urlice}");
            icecream = await GetTicketAsync(urlice.ToString());
            ShowTicket(icecream);*/

            // User test
            Console.WriteLine("Starting login tests");
            User Dave = new User("Dave", "Buster");
            Console.WriteLine(Dave);
            var urlDave = await SignupAsync(Dave);
            Console.WriteLine("signup result: " + urlDave);
            if (urlDave == null)
            {
                Console.WriteLine("Have to login");
                urlDave = await LoginAsync(Dave);
            }
            

            
/*            // Fetch all existing category records.
            var categories = await GetAllCategories();
            foreach (var cate in categories)
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

            Console.WriteLine(url.ToString());
            Console.WriteLine("Category record created. Please check in DB...");
            Console.WriteLine("Press <ENTER> to Update this record...");
            Console.ReadLine();*/
            /*
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
            */
            Console.WriteLine("Press <ENTER> to exit...");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadLine();
        return;
        /*try
        {
            do
            {
                ; // put state machine here
                // The state machine helps the user navigate to through the system.
                // This well help them fill out the variables needed by the system.
                // All pages that should be hidden will require login information,
                //      as such, preventing the simplest hack into the system.
                // Page text will be kept server side. Pages for the client will be
                //      built on a string sent from the server.
                // Login variables will be sent with all pages, but not all server-side
                //      pages need to check for login information.
                //RequestNextPage(string? user, string? pass, int currentpage, string userinput);
                // current page tell server what to do with user input
                int nextpage = 333; // The server tells which page is next (helps sm navigate).
                                    // It will be returned with
                string pagetext = ""; // server sends this text
                                      // The state machine will have to be server side?
                                      // The exact page won't matter as long as login authorized
                                      // The sssm will use current page to decide how to parse input
                                      // The sssm tells client how many inputs it needs
                                      // prompts tell client what to put in each input
                                      // Current page/state is just the url, so sssm unecessary
                                      // JSON from the client to server
                                      //  {
                                      //      "username": "user",
                                      //      "password": "pass",
                                      //      "currentpage": ##7, // just the url
                                      //      "input": "1", // for option 1 - eg login
                                      //          // so the input url is local/1
                                      //          // but the received url is local/login
                                      //          // or
                                      //          // input local/2
                                      //          // receive local/signup
                                      //  }

            } while (1 == 1);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }*/

    }


    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Start Category
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
        Console.WriteLine(category);
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
    // End Category
    //=================================
    // Start Tickets

    static void ShowTicket(Ticket t)
    {
        Console.WriteLine($"ID: {t.RequestID}\t Author: {t.AuthorID}\t" +
            $"Message: {t.Message}\t Status: {t.Status.ToString()}");
    }

    static async Task<Uri> CreateTicketAsync(Ticket t)
    {
        Console.WriteLine(t);
        HttpResponseMessage response = await client.PostAsJsonAsync("ticketplease", t);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }

    static async Task<List<Ticket>> GetAllTickets()
    {
        List<Ticket> TicketList = new List<Ticket>();
        var path = "hedidnthavehisticket";
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            TicketList = await response.Content.ReadAsAsync<List<Ticket>>();
        }
        return TicketList;
    }

    static async Task<Ticket> GetTicketAsync(string path)
    {
        Ticket t = null;
        HttpResponseMessage response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            t = await response.Content.ReadAsAsync<Ticket>();
        }
        return t;
    }
    // End Ticket
    //======================================
    // Start User
    static async Task<Uri> SignupAsync(User u)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("signup", u);
        if (response.IsSuccessStatusCode)
        {
            u = await response.Content.ReadAsAsync<User>();
        }
        else u = null;
        return response.Headers.Location;
    }

    static async Task<Uri> LoginAsync(User u)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "login", u);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("login success");
            // return URI of the created resource.
            return response.Headers.Location;
        }
        else
        {
            Console.WriteLine("Login failed");
            return null;
        }
    }
    // End User
    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
}
