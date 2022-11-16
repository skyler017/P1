using System;
using System.IO;
using RSTS.DataInfrustructure;
using Interface;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using RSTS.Navigation;
using System.Net.Sockets;
using Userzone;
using static Userzone.User;

namespace ERS;

class Program
{
    static void Main(string[] args)
    {
        // Get the db connection string
        string connectionstring = File.ReadAllText(@"C:/Users/TOWER/Desktop/revrev/P1/RSTS.DataInfrustructure/RSTS.connectionstring");
        ////////
        User who = new User("name", "pass");
        SQLLink where = new SQLLink(connectionstring);
        User who2 = where.CreateUser(who.userName, who.password);
        ConsoleBased doit = new ConsoleBased();
        doit.DisplayPage(new string[] { who2.ToString()});
        Invoice Ticket = new Invoice(who2.UserId,43,"why");
        bool what = where.SendOffInvoice(Ticket);
        if (what) doit.DisplayPage(new string[] { who2.ToString() });
        else doit.DisplayPage(new string[] { "nope" });
        Invoice please = where.RetrieveMostRecentInvoice(who2.UserId);
        doit.DisplayPage(new string[] { please.ToString() });

        ////////
        // Activate the ticketing program
        Ticketing system = new Ticketing(connectionstring);
        //system.PageNavigation(); // contains a while(true) loop

        // arrive here if there's an error with the program
        ConsoleBased CB = new ConsoleBased();
        CB.DisplayPage(new string[] { "Error: Program caused a return" });
    }
}