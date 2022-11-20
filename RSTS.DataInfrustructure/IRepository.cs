using System;
using RSTS.Userzone;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/

namespace RSTS.DataInfrustructure;

public interface ITicketRepository// where T_RSTS : class
{
    // Contract Methods
    //public List<Ticket> GetAll();
    //public List<Ticket> GetAll(int authorid);
    public List<Ticket> GetAll(int? authorid);
    public List<Ticket> GetAllOnApproved(bool approval);
    public Ticket Get(int id);
    public Ticket Create(Ticket halfdefined);
    public void Update(int id, Ticket fullydefined);
    public void Delete(int id);

}

public interface IUserRepository
{
    // Contract Methods
    public List<User> GetAll();
    public User Get(string username, string password);
    public User Get(int id);
    public User Create(User halfdefined);
    public void Update(int id, User fullydefined);
    public void Delete(int id);

}
