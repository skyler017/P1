using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RSTS.Userzone;

namespace RSTS.DataInfrustructure;


// CRUD operations on the Ticket object and the db
public class TicketRepository
{
    private readonly string _connectionstring;

    public TicketRepository(string connectionstring)
    {
        _connectionstring = connectionstring;
    }

    public List<Ticket> GetAll()
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" RequestID, AuthorID, Amount, Message, RequestDate, ApprovalStatus ");
        cmdText.Append(" FROM RSTS.Tickets ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        using SqlDataReader reader = cmd.ExecuteReader();

        List<Ticket> AllTickets = new List<Ticket>();
        while (reader.Read())
        {
            Ticket t = new Ticket();
            t.RequestID = Int32.Parse(reader["RequestID"].ToString());
            t.AuthorID = Int32.Parse(reader["AuthorID"].ToString());
            t.Amount = Int32.Parse(reader["Amount"].ToString());
            t.Message = reader["Message"].ToString();
            t.RequestDate = DateTime.Parse(reader["RequestDate"].ToString());
            t.Status = (Ticket.Approval)Int32.Parse(reader["ApprovalStatus"].ToString());

            AllTickets.Add(t);
        }
        reader.Close();
        cmd.Dispose();
        return AllTickets;
    }


    public Ticket Get(int requestID)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        // retrieve a ticket from the db
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append(" SELECT ");
        cmdText.Append(" RequestID, AuthorID, Amount, Message, RequestDate, ApprovalStatus ");
        cmdText.Append(" FROM RSTS.Tickets ");
        cmdText.Append(" WHERE RequestID = @RID ");
        using SqlCommand cmd = new SqlCommand(cmdText.ToString(), connection);
        cmd.Parameters.AddWithValue("@RID", requestID);
        using SqlDataReader reader = cmd.ExecuteReader();

        // Parse the returned data table
        Ticket t = new Ticket();
        while (reader.Read())
        {
            t.RequestID = Int32.Parse(reader["RequestID"].ToString());
            t.AuthorID = Int32.Parse(reader["AuthorID"].ToString());
            t.Amount = Int32.Parse(reader["Amount"].ToString());
            t.Message = reader["Message"].ToString();
            t.RequestDate = DateTime.Parse(reader["RequestDate"].ToString());
            t.Status = (Ticket.Approval)Int32.Parse(reader["ApprovalStatus"].ToString());
        }
        reader.Close();
        cmd.Dispose();

        return t;
    }

    public Ticket Create(Ticket t)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
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

        // retrieve the returned identity pk_requestid
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        t.RequestID = newId;
        return t;
    }

    public void Update(int OldVersionID, Ticket UpdatedTicket)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();

        string cmdText = @" UPDATE RSTS.Tickets SET " +
            " AuthorID = @userid " +
            " , Amount = @amount " +
            " , Message = @message " +
            " , RequestDate = SYSUTCDATETIME() " +
            " , ApprovalStatus  = @approval " +
            " WHERE RequestID = @RID ";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@UID", UpdatedTicket.AuthorID);
        cmd.Parameters.AddWithValue("@amount", UpdatedTicket.Amount);
        cmd.Parameters.AddWithValue("@message", UpdatedTicket.Message);
        cmd.Parameters.AddWithValue("@approval", Ticket.Approval.Pending);
        cmd.Parameters.AddWithValue("@RID", OldVersionID);
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        return;
    }

    public void Delete(int id)
    {
        using SqlConnection connection = new SqlConnection(_connectionstring);
        connection.Open();
        string cmdText = " DELETE FROM RSTS.Tickets" +
                " WHERE RequestID = @RID";
        using SqlCommand cmd = new SqlCommand(cmdText, connection);
        cmd.Parameters.AddWithValue("@RID", id);
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        return;
    }
}
