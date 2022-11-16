using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Userzone;

namespace RSTS.DataInfrustructure;

internal class TicketRepository
{
    private readonly string _connectionstring;

    public TicketRepository(string connectionstring)
    {
        _connectionstring = connectionstring;
    }

    public List<Ticket> GetAllTickets()
    {

        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" AuthorID, Amount, Message, RequestId, RequestDate, ApprovalStatus ");
        cmdText.Append(" FROM RSTS.Tickets ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);

        using SqlDataReader reader = cmd.ExecuteReader();

        List<Ticket> AllTickets = new List<Ticket>();
        while (reader.Read())
        {
            Ticket t = new Ticket();
            t.AuthorID = reader["AuthorID"].ToString();
            t.Amount = Int32.Parse(reader["Amount"].ToString());
            t.Message = reader["Message"].ToString();
            t.RequestID = Int32.Parse(reader["RequestId"].ToString());
            t.RequestDate = DateTime.Parse(reader["RequestDate"].ToString());
            t.Status = (Ticket.Approval)Int32.Parse(reader["ApprovalStatus"].ToString());

            AllTickets.Add(t);
        }
        reader.Close();
        cmd.Dispose();
        return AllTickets;
    }

    public Ticket GetTicketbyId(int requestID)
    {
        // retrieve a ticket from the db
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" AuthorID, Amount, Message, RequestId, RequestDate, ApprovalStatus ");
        cmdText.Append(" FROM RSTS.Tickets ");
        cmdText.Append(" WHERE RequestId = @RID ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), _connectionstring);
        cmd.Parameters.AddWithValue("@RID", requestID);
        using SqlDataReader reader = cmd.ExecuteReader();

        // Parse the returned data table
        while (reader.Read())
        {
            Ticket t = new Ticket();
            t.AuthorID = reader["AuthorID"].ToString();
            t.Amount = reader["Amount"].ToInt();
            t.Message = reader["Message"].ToString();
            t.RequestID = reader["RequestId"].ToInt();
            t.RequestDate = reader["RequestDate"].ToDateTime();
            t.Status = (Invoice.Approval)reader["ApprovalStatus"].ToInt();
        }
        reader.Close();
        cmd.Dispose();

        return t;
    }

    public Ticket CreateTicket(Ticket t)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        // Add ticket to db
        string cmdText =
            @"INSERT INTO RSTS.Tickets(AuthorID, Amount, Message, RequestDate, ApprovalStatus) " +
                "VALUES " +
                "(@UID,@amount,@message,SYSUTCDATETIME(),@approval); " +
                "SELECT @@IDENTITY;";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@UID", t.AuthorID);
        cmd.Parameters.AddWithValue("@amount", t.Amount);
        cmd.Parameters.AddWithValue("@message", t.Message);
        cmd.Parameters.AddWithValue("@approval", Ticket.Approval.Pending);
        cmd.ExecuteNonQuery();
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        t.RequestID = newId;
        return t;
    }

    public void Update(string connString, int id, Ticket category)
    {
        using (SqlConnection connection = new SqlConnection(connString))
        {
            StringBuilder qry = new StringBuilder();
            qry.Append(" UPDATE Categories");
            qry.Append($" SET CategoryName = '{category.CategoryName}'");
            qry.Append($" , Description = '{category.Description}'");
            qry.Append($" WHERE CategoryId = {id}");
            Console.WriteLine(qry.ToString());

            SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
            connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        return;
    }

    public void Delete(string connString, int id)
    {
        using (SqlConnection connection = new SqlConnection(connString))
        {
            StringBuilder qry = new StringBuilder();
            qry.Append(" DELETE FROM Categories");
            qry.Append($" WHERE CategoryId = {id}");
            Console.WriteLine(qry.ToString());

            SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
            connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        return;
    }
}
