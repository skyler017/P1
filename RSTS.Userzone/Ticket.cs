using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTS.Userzone
{ 
    public class Ticket
    {
        // Fields
        public int RequestID { get; set; }
        public int AuthorID { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public DateTime RequestDate { get; set; }
        public enum Approval { Pending=0, Denied=-1, Approved=1 };
        public Approval Status;


        // Constructor
        /// <summary>
        /// This overload is the empty Ticket that gets filled by the database.
        /// </summary>
        public Ticket() { }

        /// <summary>
        /// This overload is the completed Ticket returned by the database.
        /// </summary>
        /// <param name="authorid"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public Ticket(int authorid, int amount, string message, 
            int requestid, DateTime requestdate, Approval status)
        {
            AuthorID = authorid;
            Amount = amount;
            Message = message;
            RequestID = requestid;
            RequestDate = requestdate;
            Status = status;
        }

        /// <summary>
        /// This overload is the partial Ticket created by the user.
        /// </summary>
        /// <param name="authorid"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        public Ticket(int authorid, int amount, string message)
        {
            AuthorID = authorid;
            Amount = amount;
            Message = message;
        }

        // Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Requested by userid "+ AuthorID);
            sb.AppendLine("Requested on " + RequestDate);
            sb.AppendLine("Requested amount: $"+Amount);
            sb.AppendLine("Requested id: " + RequestID);
            sb.AppendLine("status: " + Status.ToString());
            sb.AppendLine(Message);
            return sb.ToString();
        }
        
    }
}
