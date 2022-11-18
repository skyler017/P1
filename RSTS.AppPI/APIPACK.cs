using RSTS.AppPI;
using RSTS.DataInfrustructure;
using RSTS.Userzone;

Console.WriteLine("Hello, World!");
string connectionstring = @"C:/Users/TOWER/Desktop/revrev/P1/RSTS.DataInfrustructure/RSTS.connectionstring";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connValue = File.ReadAllText(connectionstring);
//var connValue = builder.Configuration.GetValue<string>("ConnectionString:NorthwindDB");
TicketRepository TicketRoll = new TicketRepository(connValue);
UserRepository UserRoll = new UserRepository(connValue);


builder.Services.AddTransient<CategoryRepository>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// Tickets start here
//https://localhost:7152/ticketplease
app.MapGet("/hedidnthavehisticket", () =>
    TicketRoll.GetAll());

app.MapPost("/ticketplease", (Ticket t) =>
{
    t = TicketRoll.Create(t);
    return Results.Created($"/ticketplease/{t.RequestID}", t);
});

//https://localhost:7152/ticket/1
app.MapGet("/ticketplease/{id}", (int id) =>
    TicketRoll.Get(id));
// End Tickets
//=================================
// Users start here
app.MapPost("/signup", (User u) =>
{
    Console.WriteLine("...Within Signup API with...");
    Console.WriteLine(u);
    User checker = UserRoll.Get(u.Username, u.Password);
    Console.WriteLine(u);
    if (checker.Username != null)
    {
        return Results.Conflict(u);
    }
    else
    {
        u = UserRoll.Create(u);
        return Results.Created($"/user/{u.UserID}", u);
    }
});

app.MapPost("/login", (User u) =>
{
    Console.WriteLine("...Within Login API...");
    Console.WriteLine(u);
    u = UserRoll.Get(u.Username, u.Password);
    Console.WriteLine(u);
    switch (u.EmployeeType)
    {
        case User.Role.Employee:
            Console.WriteLine("signin employee");
            return Results.Created($"/user/{u.UserID}", u);
            break;
        case User.Role.Manager:
            Console.WriteLine("signin manager");
            return Results.Created($"/manager/{u.UserID}", u);
            break;
        default:
            Console.WriteLine("Error: Login error");
            return Results.BadRequest(u);
    }
});

// End Users
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
// Start Category
//https://localhost:7152/categories/
app.MapGet("/categories", (CategoryRepository repo) =>
    repo.GetAll(connValue));

//https://localhost:7152/categories/1
app.MapGet("/categories/{id}", (int id, CategoryRepository repo) =>
    repo.Get(connValue, id));

//https://localhost:7152/categories/
//{
//    "categoryid": 0,
//    "categoryName": "test",
//    "description": "Test"
//}
app.MapPost("/categories", (Category category, CategoryRepository repo) =>
{
    Console.WriteLine(">>>>" + category);
    category = repo.Create(connValue, category);
    return Results.Created($"/categories/{category.Categoryid}", category);
});

//https://localhost:7152/categories/14
app.MapPut("/categories/{id}", (int id, Category category, CategoryRepository repo) =>
{
    repo.Update(connValue, id, category);
    return Results.NoContent();
});

//https://localhost:7152/categories/14
app.MapDelete("/categories/{id}", (int id, CategoryRepository repo) =>
{
    repo.Delete(connValue, id);
    return Results.Ok(id);
});


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}