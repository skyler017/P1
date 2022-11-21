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


/// post-ticket
/// <summary>
/// Adds the ticket t to the database.
/// </summary>
/// <returns>
/// The filled ticket object after its added to the db: code 201 Created (to pass object)
/// A Null object because some fields are blank: code 409 Conflict
/// </returns>
// used by employees to create a new ticket
app.MapPost("/ticket", (Ticket t) =>
{
    Ticket DBt = Accessing.Try_AddTicket(t);
    if(DBt==null) Results.BadRequest(null);
    return Results.Created($"/ticket/id",DBt);
});


/// post-ticket/0
/// <return>
/// Returns the oldest ticket unapproved to the given user
/// </return>
app.MapPost("/ticket/0", (User u) =>
{
    User DBu = Accessing.LoginUser(u.Username, u.Password);
    switch(DBu.EmployeeType)
    {
        case User.Role.Manager:
            Ticket DBt = Accessing.GetOldestUnapprovedTicket();
            if (DBt != null) return Results.Created($"/ticket/{DBt.RequestID}", DBt);
            else return Results.BadRequest();
        default:
            return Results.Unauthorized();
    }
    
});


// For the employee to edit a ticket, or manager to approve it.
// The user needs to have a call to get this ticket before making this api call.
app.MapPost("/ticket/{id}", (int id, Ticket t) =>
{
    //Console.WriteLine(t);
    Accessing.UpdateTicket(t);
    return Results.Ok("/tickets/{id}");
});

/// post-tickets
/// <returns>
/// All tickets available to the user : code 201 Created (to pass object)
/// </returns>
app.MapPost("/tickets", (User u) =>
{
    List<Ticket> tickets;
    //Console.WriteLine(u);
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
});

// used by the user or managers to see a specific ticket
/*app.MapGet("/tickets/{id}", (int id) =>
{
    Ticket t = Accessing.GetATicket(id);
    if (t == null) return Results.Forbid();
    return Results.Ok(t);
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

//////////////////////////////
/////     End of API     /////
//////////////////////////////
app.Run();
