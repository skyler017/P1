using RSTS.AppPI;
using RSTS.Business;
using RSTS.Userzone;


/// <summary>
/// API of the backend. Organizes communication between the client and the database.
/// Does not call on the database directly, but through the use of Business project,
/// which contains the BusinessLayer class.
/// </summary>


//////////////////////////////
/////   DB Connection    /////
//////////////////////////////
///
Console.WriteLine("Hello, World!");
string connectionstring = @"C:/Users/TOWER/Desktop/revrev/P1/RSTS.DataInfrustructure/RSTS.connectionstring";
var connValue = File.ReadAllText(connectionstring);
IDbContext Accessing = new BusinessLayer(connValue);




//////////////////////////////
/////      Builder       /////
//////////////////////////////
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddTransient<BusinessLayer>();
//builder.Services.AddTransient<IDbContext, BusinessLayer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


//////////////////////////////
/////     Ticket API     /////
//////////////////////////////

// used to see a subset of tickets
/*app.MapGet("/tickets", (User u) =>
{
    List<Ticket> tickets;
    Console.WriteLine(u);
    // check creds
    User DBu = Accessing.LoginUser(u.Username,u.Password);
    // check emp type
    switch (DBu.EmployeeType)
    {
        case User.Role.Employee:
            tickets = Accessing.GetAllofUsersTickets(u);
            return Results.Ok(tickets);
        case User.Role.Manager:
            tickets = Accessing.GetAllUnapprovedTickets(u);
            return Results.Ok(tickets);
        default:
            return Results.BadRequest();
    }
});*/

// used by employees to create a new ticket
/*app.MapPost("/tickets", (User u, Ticket t) =>
{
    return Results.Problem();
});*/

//https://localhost:7152/ticket/1
// used by the user or managers to see a specific ticket
/*app.MapGet("/tickets/{id}", (User u, int ticketid) =>
{
    Ticket t = Accessing.GetATicket(u, ticketid);
    if (t == null) return Results.Forbid();
    return Results.Ok(t);
});
*/
// requested by the user to edit a ticket, or manager to approve it
/*app.MapPost("/tickets/{id}", (User u, int ticketid) =>
{
    return Results.BadRequest();
});*/
// End Tickets
//============================


//////////////////////////////
/////     Login API      /////
//////////////////////////////

/// <summary>
/// Adds the user to the database, if the user does not already exist.
/// </summary>
/// <returns>
/// A User object. User gets added to the db: code 201 Created (to pass object)
/// A Null Object. User is already in the db: code 409 Conflict
/// </returns>
app.MapPost("/signup", (User u) =>
{
    //Console.WriteLine("...Within Signup API with...");
    User DBu = Accessing.Try_AddUser(u);
    Console.WriteLine(">>>APIsignup<<< "+DBu);
    if (DBu != null) 
        return Results.Created($"/user/{DBu.UserID}",DBu);
    else
        return Results.Conflict(null);
});

/// <summary>
/// Retrievs a full user object, if the provided credentials are correct.
/// </summary>
/// <returns>
/// Path to the user page if credentials match: code 201 Created (to pass object)
/// No path. Creadentials do not match records: code 401 Unauthorized
/// </returns>
app.MapPost("/login", (User u) =>
{
    if (u == null) return Results.Unauthorized();
    User DBu = Accessing.LoginUser(u.Username, u.Password);
    if (DBu == null) return Results.Unauthorized();
    switch (DBu.EmployeeType)
    {
        case User.Role.Employee:
            Console.WriteLine("signin employee");
            return Results.Created($"/user/{DBu.UserID}",DBu);
            //break;
        case User.Role.Manager:
            Console.WriteLine("signin manager");
            return Results.Created($"/manager/{DBu.UserID}", DBu);
            //break;
        default:
            Console.WriteLine("Error: Login error");
            return Results.Unauthorized();
    }
});

// returns 200 OK for name available, or 409 Conflict for unavailable
app.MapPost("/check", (User u) =>
{
    //Console.WriteLine("API check on " + u.Username);
    bool used = Accessing.IsUsernameUsed(u.Username);
    //Console.WriteLine(used);
    return used ? Results.Conflict() : Results.Ok();
});
// End Login
// <<<<<<<<<<<<<<<<<<<<<<<<<<<
// Start Category
//https://localhost:7152/categories/
//app.MapGet("/categories", (CategoryRepository repo) =>
//    repo.GetAll(connValue));

//https://localhost:7152/categories/1
//app.MapGet("/categories/{id}", (int id, CategoryRepository repo) =>
//    repo.Get(connValue, id));

//https://localhost:7152/categories/
//{
//    "categoryid": 0,
//    "categoryName": "test",
//    "description": "Test"
//}
/*app.MapPost("/categories", (Category category, CategoryRepository repo) =>
{
    Console.WriteLine(">>>>" + category);
    category = repo.Create(connValue, category);
    return Results.Created($"/categories/{category.Categoryid}", category);
});*/

//https://localhost:7152/categories/14
/*//app.MapPut("/categories/{id}", (int id, Category category, CategoryRepository repo) =>
{
    repo.Update(connValue, id, category);
    return Results.NoContent();
});*/

//https://localhost:7152/categories/14
/*app.MapDelete("/categories/{id}", (int id, CategoryRepository repo) =>
{
    repo.Delete(connValue, id);
    return Results.Ok(id);
});*/


//////////////////////////////
/////     End of API     /////
//////////////////////////////
app.Run();
