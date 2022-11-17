using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;
using RSTS.Userzone;

namespace RSTS.DataInfrustructure;

public class SQLLink : IRepository
{
    // Fields
    private readonly string _connectionString;

    // Constructor
    public SQLLink(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    // Methods
    public bool CheckUserExists(string username, string password)
    {
        return false;
    }

    public User RetrieveUser(string username, string password)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // retrieve the completed user back from the db
        string cmdText =
            @"SELECT Username, Password, UserID, EmployeeType " +
                "FROM RSTS.Users " +
                "WHERE Username = @name;";
        using SqlCommand cmd2 = new SqlCommand(cmdText, connection);
        cmd2.Parameters.AddWithValue("@name", username);
        using SqlDataReader reader = cmd2.ExecuteReader();

        // Parse the returned data table
        User shell;
        while (reader.Read())
        {
            string name = reader.GetString(0);
            string pass = reader.GetString(1);
            int userid = reader.GetInt32(2);
            User.Role type = (User.Role)reader.GetInt32(3);
            return shell = new User(name, pass, userid, type);
        } // The SQLDataReader does not need to close because it was made temporary to begin with

        connection.Close();
        return null;
    }

    public User CreateUser(string username, string password)
    {
        // connect to db
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // Add user to db
        string cmdText =
            @"INSERT INTO RSTS.Users(Username, Password, EmployeeType) " +
                "VALUES " +
                "(@name,@pass,@role);";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@name", username);
        cmd.Parameters.AddWithValue("@pass", password);
        cmd.Parameters.AddWithValue("@role", User.Role.Employee);
        cmd.ExecuteNonQuery();
        connection.Close();

        // Check the user is the same one created
        User shell = RetrieveUser(username, password);
        return shell;
    }

    public Ticket RetrieveMostRecentInvoice(int userid )
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // retrieve the completed ticket back from the db
        string cmdText =
            @"SELECT TOP 1 AuthorID, Amount, Message, RequestId, RequestDate, ApprovalStatus " +
                "FROM RSTS.Tickets " +
                "WHERE AuthorID = @UID" +
                "ORDER BY RequestId DESC;";
        using SqlCommand cmd2 = new SqlCommand(cmdText, connection);
        cmd2.Parameters.AddWithValue("@UID", userid);
        using SqlDataReader reader = cmd2.ExecuteReader();

        // Parse the returned data table
        Ticket? shell = null;
        while (reader.Read())
        {
            int authorid = reader.GetInt32(0);
            int amount = reader.GetInt32(1);
            string message = reader.GetString(2);
            int requestid = reader.GetInt32(3);
            DateTime requestdate = reader.GetDateTime(4);
            Ticket.Approval status = (Ticket.Approval)reader.GetInt32(5);
            shell = new Ticket(authorid, amount, message, requestid, requestdate, status);
            return shell;
        } // The SQLDataReader does not need to close because it was made temporary to begin with


        return null;
    }

    public bool SendOffInvoice(Ticket t)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // Add ticket to db
        string cmdText =
            @"INSERT INTO RSTS.Tickets(AuthorID, Amount, Message, RequestDate, ApprovalStatus) " +
                "VALUES " +
                "(@UID,@amount,@message,SYSUTCDATETIME(),@approval);";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@UID", t.AuthorID);
        cmd.Parameters.AddWithValue("@amount", t.Amount);
        cmd.Parameters.AddWithValue("@message", t.Message);
        cmd.Parameters.AddWithValue("@approval", Ticket.Approval.Pending);
        cmd.ExecuteNonQuery();

        Ticket shell = RetrieveMostRecentInvoice(t.AuthorID);

        if (shell == null || 
            shell.AuthorID != t.AuthorID ||
            shell.Amount != t.Amount ||
            shell.Message != t.Message)
            return false;
        return true;
    }


    public void startTabling()
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // Add ticket to db
        string cmdText =
            @"CREATE TABLE RSTS.Tickets " +
            "( " +
            "AuthorID INTEGER NOT NULL, " +
            "Amount INT NOT NULL, " +
            "Message NVARCHAR(MAX), " +
            "RequestId INT IDENTITY PRIMARY KEY, " +
            "RequestDate DATETIME NOT NULL, " +
            "ApprovalStatus INT, " +
            ");";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.ExecuteNonQuery();
    }
}

/*
private class what {
    public class SqlRepository : IRepository
    {
        // will hold all of the communication to and from the database

        // Fields
        private readonly string _connectionString;

        // Constructor
        public SqlRepository(string connectionString)
        {
            this._connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // Methods
        public IEnumerable<Teacher> GetAllTeachers()
        {
            List<Teacher> result = new();

            using SqlConnection connection = new SqlConnection(this._connectionString);
            connection.Open();

            using SqlCommand cmd = new(
                "Select Teacher_ID, Name " +
                "FROM School.Teacher;", connection);

            // index     0     1     2     3
            // column   id    name
            // row 1    6     Tryg
            // row 2    7     Andrew

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int ID = reader.GetInt32(0);
                string Name = reader.GetString(1);
                result.Add(new(ID, Name));
            }
            // reader.??? is parsing the response form the database to C# datatypes

            connection.Close();
            return result;
        }


        public Teacher CreateNewTeacher(string Name)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string cmdText =
                @"INSERT INTO School.Teacher (Name)
                VALUES
                (@name);";

            using SqlCommand cmd = new SqlCommand(cmdText, connection);

            cmd.Parameters.AddWithValue("@name", Name);

            cmd.ExecuteNonQuery();

            cmdText =
                @"SELECT Teacher_ID, Name
                FROM School.Teacher
                WHERE Name = @name;";

            using SqlCommand cmd2 = new SqlCommand(cmdText, connection);

            cmd2.Parameters.AddWithValue("@name", Name);

            using SqlDataReader reader = cmd2.ExecuteReader();

            Teacher tmpTeacher;
            while (reader.Read())
            {
                return tmpTeacher = new Teacher(reader.GetInt32(0), reader.GetString(1));
            }
            connection.Close();
            Teacher noTeacher = new();
            return noTeacher;
        }


        public string GetStudentName(int ID)
        {
            string? name = "";
            using SqlConnection connection = new SqlConnection(this._connectionString);
            connection.Open();

            string cmdText = @"SELECT Name
                FROM School.Student
                WHERE Student_ID = @ID;";

            using SqlCommand cmd = new SqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@ID", ID);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                name = reader.GetString(0);
            }

            connection.Close();

            if (name != null)
            { return name; }
            return null;
        }
    }
}
*/