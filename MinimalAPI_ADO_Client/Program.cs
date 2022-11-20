using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Interface;
using System.Text;
using RSTS.Userzone;

// Running the following command in NuGet package manager console is
// necessary to run this project:
/// Install-Package Microsoft.AspNet.WebApi.Client
// Alternatively, navigate NuGet to find the package.


namespace MinimalAPI_ADO_Client;

class Program
{
    static HttpClient client = new HttpClient();

    private enum Page
    {
        OPEN = 0
        , SIGNU_E, SIGNU_A, SIGNU_P, SIGNU_V // Signup for an account - E_mail, A_vailable, P_assword, V_alidate
        , LOGIN_E,          LOGIN_P, LOGIN_V // Login to existing account
        , WELCOME, LAND_EM, LAND_MN
        , EVW_TKT, EFT_TKT, CRT_TKT // Employee side: view, filter, create
        , MVW_TKT, MFT_TKT, APV_TKT // Manager side: view, filter, approve
    }
    private enum CMD
    {
        NIL = -1
        , OPT1 = 1, OPT2 = 2, OPT3 = 3, OPT4 = 4 /**/, OPT9 = 9
        , FORW, BACK, FAIL, RETRY
    };


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
            // User test
            /*            Console.WriteLine("Starting login tests");
                        User Dave = new User("Dave", "Buster");
                        Console.WriteLine(Dave);
                        var urlDave = await SignupAsync(Dave);
                        Console.WriteLine("signup result: " + urlDave);
                        if (urlDave == null)
                        {
                            Console.WriteLine("Have to login");
                            urlDave = await LoginAsync(Dave);
                        }*/

            // Manager test
            /*User BigR = new User("R@hotmail", "kitty");
            var urlBigR = await LoginAsync(BigR);*/

            // All tickets
            /*var tickets = await GetAllTickets();
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


            // Fetch all existing category records.
            /*var categories = await GetAllCategories();
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
            
            // Update the Category
            /*Console.WriteLine("Updating description...");
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

        try
        {
            Page Bookmark = Page.OPEN;
            CMD Orders;
            User employee = new User();
            string GivenUsername = "";
            string GivenPassword = "";
            //string webpath = "";

            while (1 == 1)
            {
                Orders = CMD.NIL;
                switch (Bookmark)
                {
                    case Page.OPEN:
                        OPEN_WelcomeText();
                        Orders = OPEN_ProcessInput();
                        switch (Orders)
                        {
                            case CMD.OPT1: // Existing User
                                Bookmark = Page.LOGIN_E;
                                break;
                            case CMD.OPT2: // New User
                                Bookmark = Page.SIGNU_E;
                                break;
                            case CMD.RETRY: // input error
                                continue;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                return;
                        }
                        break;

                    case Page.SIGNU_E:
                        SIGNU_E_Prompt();
                        Orders = LOGIN_E_ProcessInput(out GivenUsername); // LOGIN is used rather than SIGNU because it still works
                        switch (Orders)
                        {
                            case CMD.OPT9: // Go back
                            case CMD.BACK:
                                Bookmark = Page.OPEN;
                                break;
                            case CMD.FORW: // Email is formatted correctly
                                Bookmark = Page.SIGNU_A;
                                break;
                            case CMD.RETRY: // Input error
                                continue;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                return;
                        }
                        break;

                    case Page.SIGNU_A:
                        Orders = SIGNU_A_ProcessUsername(GivenUsername);
                        switch(Orders)
                        {
                            case CMD.FORW: // name is available
                                Bookmark = Page.SIGNU_P;
                                break;
                            case CMD.RETRY: // name is taken
                                Bookmark = Page.SIGNU_E;
                                break;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                break;
                        }
                        break;

                    case Page.SIGNU_P:
                        SIGNU_P_Prompt();
                        Orders = LOGIN_P_ProcessInput(out GivenPassword); // LOGIN is used rather than SIGNU because it still works
                        switch (Orders)
                        {
                            case CMD.OPT9: // Go back
                            case CMD.BACK:
                                Bookmark = Page.OPEN;
                                continue;
                            case CMD.FORW: // Password acceptable
                                Bookmark = Page.SIGNU_V;
                                break;
                            case CMD.FAIL: // Something about the password is bad
                                Bookmark = Page.SIGNU_P;
                                break;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                break;
                        }
                        break;

                    case Page.SIGNU_V:
                        Orders = SIGNU_V_ValidateLogin(GivenUsername, GivenPassword, out employee);
                        switch (Orders)
                        {
                            case CMD.FORW: // Account creation successful
                                if (employee == null) return;
                                Bookmark = Page.WELCOME;
                                break;
                            case CMD.FAIL: // Could not create account for some reason
                                Bookmark = Page.OPEN;
                                break;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                return;
                        }
                        break;

                    case Page.LOGIN_E:
                        LOGIN_E_Prompt();
                        Orders = LOGIN_E_ProcessInput(out GivenUsername);
                        switch (Orders)
                        {
                            case CMD.OPT9: // Go back
                            case CMD.BACK:
                                Bookmark = Page.OPEN;
                                break;
                            case CMD.FORW: // Email received
                                Bookmark = Page.LOGIN_P;
                                break;
                            //case CMD.RETRY: // Input error
                                //continue;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                return;
                        }
                        break;

                    case Page.LOGIN_P:
                        LOGIN_P_Prompt();
                        Orders = LOGIN_P_ProcessInput(out GivenPassword);
                        switch (Orders)
                        {
                            case CMD.OPT9: // Go back
                            case CMD.BACK:
                                Bookmark = Page.LOGIN_E;
                                continue;
                            case CMD.FORW: // Password received
                                Bookmark = Page.LOGIN_V;
                                break;
                            //case CMD.FAIL: // Something about the password is bad
                            //    Bookmark = Page.LOGIN_E;
                            //    break;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                break;
                        }
                        break;

                    case Page.LOGIN_V:
                        Orders = LOGIN_V_ValidateLogin(GivenUsername, GivenPassword, out employee);
                        switch (Orders)
                        {
                            case CMD.FORW: // Login successful
                                Bookmark = Page.WELCOME;
                                break;
                            case CMD.FAIL: // Credentials not found
                                Bookmark = Page.LOGIN_E;
                                break;
                            default:
                                NESTED_DEFAULT_ERROR(Bookmark, Orders);
                                return;
                        }
                        break;

                    case Page.WELCOME:
                        if (employee == null)
                        {
                            Console.WriteLine("Employee Token should not be null");
                            return;
                        }
                        else
                        {
                            //Console.WriteLine(employee);
                            switch(employee.EmployeeType)
                            {
                                case User.Role.Employee:
                                    Bookmark = Page.LAND_EM;
                                    break;
                                case User.Role.Manager:
                                    Bookmark = Page.LAND_MN;
                                    break;
                                default:
                                    NESTED_DEFAULT_ERROR(Bookmark, CMD.OPT1);
                                    return;
                            }
                        }
                        break;

                    case Page.LAND_EM:
                        LANDING_Employee_WelcomeText(employee);
                        Bookmark = Page.OPEN;
                        break;
                    case Page.LAND_MN:
                        LANDING_Manager_WelcomeText(employee);
                        Bookmark = Page.OPEN;
                        break;
                    default:
                        PageDoesNotExist(Bookmark);
                        return;
                }
            }
            // The state machine helps the user navigate to through the system.
            // This well help them fill out the variables needed by the system.
            // All pages that should be hidden will require login information,
            //      as such, preventing the simplest hack into the system.

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }


    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    ///// Shared Page resources /////
    private static readonly string NineToGoHome = "You can press 9 and 'enter' to go back.";

    private static int CheckForValidNumerical(string input, int[] valids)
    {
        int userInputNum;
        //if (string.IsNullOrEmpty(input)) return -1;
        //else 
        if (Int32.TryParse(input, out userInputNum))
        {
            if (valids.Contains(userInputNum)) return userInputNum;
            else return -1;
        }
        return -1;
    }

    ///// Pages and Processing /////

    private static void OPEN_WelcomeText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Welcome to ERS Ticketing.");
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter').");
        sb.AppendLine("1.Login");
        sb.AppendLine("2.Sign-up");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static CMD OPEN_ProcessInput()
    {
        IInput consoleBased = new ConsoleBased();

        // Fetch the user's input string and ensure it is a 1 or 2
        string userInputMessage = consoleBased.GetUserInput();
        int userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 1, 2 });
        if (userInputNum > 0) return (CMD)userInputNum;
        consoleBased.DisplayPage(new string[] { "Please enter a number from the selection." });
        return CMD.RETRY;
    }

    private static void SIGNU_E_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to create your account");
        sb.AppendLine("Please type in your full email (eg: name@demo.com) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static CMD SIGNU_A_ProcessUsername(string username)
    {
        Console.WriteLine("looking for user " + username);
        var taken = CheckNameAsync(username);
        if (taken.Result) return CMD.FORW;
        ConsoleBased cB = new();
        cB.DisplayPage(new string[] { "That username is taken; you must use another." });
        return CMD.RETRY;
    }
    private static void SIGNU_P_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to create your account");
        sb.AppendLine("Please type in your password (eg: password) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static CMD SIGNU_V_ValidateLogin(string username, string password, out User token)
    {
        Task<User> task = SignupAsync(new User(username, password));
        token = task.Result;
        if(task == null || token == null) return CMD.FAIL;
        return CMD.FORW;
    }

    private static void LOGIN_E_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to log you in");
        sb.AppendLine("Please type in your full email (eg: name@demo.com) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static CMD LOGIN_E_ProcessInput(out string email)
    {
        email = "";
        IInput cB = new ConsoleBased();

        // Fetch the user's input string
        string userInputMessage = cB.GetUserInput();

        // check for 9
        int userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 9 });
        if (userInputNum == 9) return CMD.BACK;

        // must be an email if its made it here
        email = userInputMessage;
        return CMD.FORW;
    }
    private static void LOGIN_P_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to log you in");
        sb.AppendLine("Please type in your password (eg: password) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static CMD LOGIN_P_ProcessInput(out string password)
    {
        password = "";

        // Fetch the user's input string
        IInput cB = new ConsoleBased();
        string userInputMessage = cB.GetUserInput();

        // check for 9
        int userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 9 });
        if (userInputNum == 9) return CMD.BACK;

        // the string must be a password
        password = userInputMessage;
        return CMD.FORW;
    }

    private static CMD LOGIN_V_ValidateLogin(string username, string password, out User token)
    {
        Task<User> task = LoginAsync(new User(username, password));
        token = task.Result;
        //Console.WriteLine(token);
        if (task == null || token == null) return CMD.FAIL;
        return CMD.FORW;
    }

    private static void LANDING_Manager_WelcomeText(User manager)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Welcome back {manager.Username}.");
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Check tickets");
        sb.AppendLine("2.Register employees as managers");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private static void LANDING_Manager_ViewTickets(User Dave/*, filter method*/)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Now displaying reimbursments:");
        // sb.AppendLine(call function that gets all the approved tickets)
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Find a subset of tickets");
        sb.AppendLine("2.Approve pending tickets");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }

    private static void LANDING_Employee_WelcomeText(User employee)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Welcome {employee.Username}.");
        sb.AppendLine("Now displaying reimbursments:");
        // sb.AppendLine(call function that gets all the approved tickets)
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Find a subset of tickets");
        sb.AppendLine("2.Approve pending tickets");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }

    private static void PageDoesNotExist(Page which)
    {
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { "The requested page does not exist: " + which });
    }

    private static void NESTED_DEFAULT_ERROR(Page where, CMD why)
    {
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { $"Error on Page {where} due to Command {why}.\nExiting Program" });
    }

    //======================================
    ///// API Interaction /////
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

    //static async Task<List<Ticket>> GetAllTicketsAsync(string userpath, User u)
    static async Task<List<Ticket>> GetAllTickets()
    {
        List<Ticket> TicketList = new List<Ticket>();
        var path = "hedidnthavehisticket";
        HttpResponseMessage response = await client.GetAsync(path);
        //HttpResponseMessage response = await client.PostAsJsonAsync(userpath, u);
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
    static async Task<User> SignupAsync(User u)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("signup", u);
        //Console.WriteLine(">>>asignup<<<"+response);
        if (response.IsSuccessStatusCode)
        {
            u = await response.Content.ReadAsAsync<User>();
        }
        else u = null;
        return u; // response.Headers.Location; // "/user./{id}"
    }

    static async Task<User> LoginAsync(User u)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("login", u);
        //Console.WriteLine(">>>alogin<<<" + response);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("login success");
            u = await response.Content.ReadAsAsync<User>();

            // return URI of the created resource.
            //return response.Headers.Location; // "/user./{id}"
        }
        else
        {
            Console.WriteLine("Login failed");
            u = null;
            //return u;
        }
        return u;
    }

    static async Task<bool> CheckNameAsync(string username)
    {
        //Console.WriteLine("checking on " + username);
        HttpResponseMessage response = await client.PostAsJsonAsync("check", new User ( username, null));
        //Console.WriteLine(response);
        if (response.IsSuccessStatusCode)
            return true;
        else
            return false;
    }
    // End User
    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
}
