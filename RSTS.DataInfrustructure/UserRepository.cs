using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSTS.Userzone;

namespace RSTS.DataInfrustructure;


// CRUD operations on the Ticket object and the db
public class UserRepository : IUserRepository
{
    private readonly string _connectionstring;

    public UserRepository(string connectionstring)
    {
        _connectionstring = connectionstring;
    }
    // User(string name, string pass, int Id, Role eType)
    public List<User> GetAll()
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" UserID, Username, Password, EmployeeType ");
        cmdText.Append(" FROM RSTS.Users ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        using SqlDataReader reader = cmd.ExecuteReader();

        List<User> AllUsers = new List<User>();
        while (reader.Read())
        {
            User u = new User
            {
                UserID = Int32.Parse(reader["UserID"].ToString()),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                EmployeeType = (User.Role)Int32.Parse(reader["ApprovalStatus"].ToString())
            };

            AllUsers.Add(u);
        }
        reader.Close();
        cmd.Dispose();
        return AllUsers;
    }

    public User Get(string username, string password)
    {
        //Console.WriteLine("Repo check on " + username + " || " + (password == "" ? "none" : password) + ". RepoGo");
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        // retrieve a ticket from the db
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" UserID, Username, Password, EmployeeType ");
        cmdText.Append(" FROM RSTS.Users ");
        cmdText.Append(" WHERE Username = @user ");
        cmdText.Append(password != "" ? " AND Password = @pass ":"");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        cmd.Parameters.AddWithValue("@user", username);
        if (password != "")
            cmd.Parameters.AddWithValue("@pass", password);
        using SqlDataReader reader = cmd.ExecuteReader();

        // Parse the returned data table
        User u = null;
        while (reader.Read())
        {
            u = new User
            {
                UserID = Int32.Parse(reader["UserID"].ToString()),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                EmployeeType = (User.Role)Int32.Parse(reader["EmployeeType"].ToString())
            };
        }
        reader.Close();
        cmd.Dispose();

        return u;
    }

    public User Get(int searchid)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        // retrieve a ticket from the db
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" UserID, Username, Password, EmployeeType ");
        cmdText.Append(" FROM RSTS.Users ");
        cmdText.Append(" WHERE UserID = @UID ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        cmd.Parameters.AddWithValue("@UID", searchid);
        using SqlDataReader reader = cmd.ExecuteReader();

        // Parse the returned data table
        User u = null;
        while (reader.Read())
        {
            u = new User
            {
                UserID = Int32.Parse(reader["UserID"].ToString()),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                EmployeeType = (User.Role)Int32.Parse(reader["EmployeeType"].ToString())
            };
        }
        reader.Close();
        cmd.Dispose();

        return u;
    }

    public User Create(User u)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        // Add ticket to db
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" INSERT INTO RSTS.Users ");
        cmdText.Append(" (Username, Password, EmployeeType) ");
        cmdText.Append(" VALUES ");
        cmdText.Append(" (@username, @password, @etype); ");
        cmdText.Append(" SELECT @@IDENTITY;");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        cmd.Parameters.AddWithValue("@username", u.Username);
        cmd.Parameters.AddWithValue("@password", u.Password);
        cmd.Parameters.AddWithValue("@etype", User.Role.Employee);
        //cmd.ExecuteNonQuery();

        // retrieve the returned identity pk_userid
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        u.UserID = newId;
        u.EmployeeType = User.Role.Employee;
        return u;
    }

    public void Update(int usertoupdateID, User UpdatedUser)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" UPDATE RSTS.Users SET ");
        cmdText.Append(" Username = @password ");
        cmdText.Append(", Password = @message ");
        cmdText.Append(", EmployeeType = @etype ");
        cmdText.Append(" WHERE UserID = @UID ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        cmd.Parameters.AddWithValue("@username", UpdatedUser.Username);
        cmd.Parameters.AddWithValue("@password", UpdatedUser.Password);
        cmd.Parameters.AddWithValue("@etype", User.Role.Employee);
        cmd.Parameters.AddWithValue("@UID", usertoupdateID);
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        return;
    }

    public void Delete(int id)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();
        string cmdText = " DELETE FROM RSTS.Users" +
                " WHERE UserID = @UID";

        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@UID", id);
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        return;
    }
}
