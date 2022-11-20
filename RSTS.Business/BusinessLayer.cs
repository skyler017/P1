using RSTS.DataInfrustructure;
using RSTS.Userzone;

namespace RSTS.Business;


/// <summary>
/// Class to be declared from API. API's sole way of accessing the DB and "RSTS.DataInfrustructure".
/// These methods ensure a set of DB operations happen together to maintain data stability.
/// In doing so it also confirms a client's authorization for accessing data.
/// </summary>
public class BusinessLayer : IDbContext
{
    // Repositories
    ITicketRepository TicketRoll;
    IUserRepository UserRoll;

    // Constructor
    public BusinessLayer(string connectionstring)
    {
        TicketRoll = new TicketRepository(connectionstring);
        UserRoll = new UserRepository(connectionstring);
    }

    // Methods
    public List<Ticket> GetAllUnapprovedTickets(User u)
    {
        return TicketRoll.GetAllOnApproved(false);
    }

    public List<Ticket> GetAllofUsersTickets(User u)
    {
        return TicketRoll.GetAll(u.UserID);
    }

    public Ticket GetATicket(User user, int id)
    {
        return TicketRoll.Get(id);
    }

    public bool GetUsersTicket(User u, int ticketID)
    {
        if (u == null) return false;
        Ticket t = TicketRoll.Get(ticketID);
        User checker = UserRoll.Get(u.Username, u.Password);
        if (checker != null && t.AuthorID == checker.UserID) return true;
        return false;
    }

    /// <summary>
    /// Obtains the full user object for the given credentials
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public User LoginUser(string username, string password)
    {
        return UserRoll.Get(username, password);
    }

    /// <summary>
    /// Attempts to add the new user to the database. Fails if the user already exists.
    /// </summary>
    /// <param name="u"></param>
    /// <returns>
    /// Returns the new User object if successful.
    /// Returns null if unable to add.
    /// </returns>
    public User Try_AddUser(User u)
    {
        if (IsUsernameUsed(u.Username)) return null;
        //User checker = UserRoll.Get(u.Username, u.Password);
        //Console.WriteLine(checker);

        // do not create a user if it already exists
        //if (checker != null)
        //    return null;
        else
            return UserRoll.Create(u);
    }

    public bool IsUsernameUsed(string username)
    {
        //Console.WriteLine("BL check on " + username);
        User u = UserRoll.Get(username, "");
        //Console.WriteLine("BL received: " + u);
        if (u == null) return false;
        return true;
    }
}