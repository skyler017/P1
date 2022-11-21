using RSTS.Userzone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTS.Business;

public interface IDbContext
{
    public List<Ticket> GetAllUnapprovedTickets(User user);
    public List<Ticket> GetAllofUsersTickets(User user);
    public Ticket GetATicket(User user, int id);
    public Ticket GetOldestUnapprovedTicket();
    public void UpdateTicket(Ticket ticket);
    public User LoginUser(string username, string password);
    public User Try_AddUser(User user);
    public bool IsUsernameUsed(string username);
}
