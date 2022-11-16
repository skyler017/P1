using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userzone;

namespace RSTS.Navigation;

internal class Ticketing
{
    // Fields
    private enum Page
    {
        OPEN = 0
        , SIGNU_E, SIGNU_A, SIGNU_P, SIGNU_V // Signup for an account - E_mail, A_vailable, P_assword, V_alidate
        , LOGIN_E, LOGIN_P, LOGIN_V // Login to existing account
        , LAND_EM, LAND_MN
        , EVW_TKT, EFT_TKT, CRT_TKT // Employee side: view, filter, create
        , MVW_TKT, MFT_TKT, APV_TKT // Manager side: view, filter, approve
    }
    private enum CMD
    {
        NIL = -1
        , OPT1 = 1, OPT2 = 2, OPT3 = 3, OPT4 = 4 /**/, OPT9 = 9
        , FORW, BACK, FAIL, RETRY
    };

    private readonly string _connectionstring;

    public Ticketing(string connectionstring)
    {
        _connectionstring = connectionstring;
    }

    public void PageNavigation()
    {
        Page Bookmark = Page.OPEN;
        CMD Orders;
        User Dave;
        string GivenUsername = "";
        string GivenPassword = "";

        while (1 == 1)
        {
            Orders = CMD.NIL;
            switch (Bookmark)
            {
                case Page.OPEN:
                    OPEN_WelcomeText();
                    Orders = OPEN_ProcessInput();
                    switch (Orders)
                    {
                        case CMD.OPT1: // Existing User
                            Bookmark = Page.LOGIN_E;
                            break;
                        case CMD.OPT2: // New User
                            Bookmark = Page.SIGNU_E;
                            break;
                        case CMD.RETRY: // input error
                            continue;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;

                case Page.SIGNU_E:
                    SIGNU_E_Prompt();
                    Orders = LOGIN_E_ProcessInput(out GivenUsername); // LOGIN is used rather than SIGNU because it still works
                    switch (Orders)
                    {
                        case CMD.OPT9: // Go back
                        case CMD.BACK:
                            Bookmark = Page.OPEN;
                            break;
                        case CMD.FORW: // Email is formatted correctly
                            Bookmark = Page.SIGNU_A;
                            break;
                        case CMD.RETRY: // Input error
                            continue;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;

                case Page.SIGNU_A:
                    Orders = SIGNU_A_CheckEmailAvailability(GivenUsername);
                    switch (Orders)
                    {
                        case CMD.FORW: // Email is availale
                            Bookmark = Page.SIGNU_P;
                            break;
                        case CMD.FAIL: // Email is already in the db
                            Bookmark = Page.OPEN;
                            break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;

                case Page.SIGNU_P:
                    SIGNU_P_Prompt();
                    Orders = LOGIN_P_ProcessInput(out GivenPassword); // LOGIN is used rather than SIGNU because it still works
                    switch (Orders)
                    {
                        case CMD.OPT9: // Go back
                        case CMD.BACK:
                            Bookmark = Page.OPEN;
                            continue;
                        case CMD.FORW: // Password acceptable
                            Bookmark = Page.SIGNU_V;
                            break;
                        case CMD.FAIL: // Something about the password is bad
                            Bookmark = Page.SIGNU_P;
                            break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            break;
                    }
                    break;

                case Page.SIGNU_V:
                    Orders = SIGNU_V_ValidateLogin(GivenUsername, GivenPassword);
                    switch (Orders)
                    {
                        case CMD.FORW: // Account creation successful
                            //Bookmark = Page.WELCOME;
                            break;
                        case CMD.FAIL: // Could not create account for some reason
                            Bookmark = Page.OPEN;
                            break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;

                case Page.LOGIN_E:
                    LOGIN_E_Prompt();
                    Orders = LOGIN_E_ProcessInput(out GivenUsername);
                    switch (Orders)
                    {
                        case CMD.OPT9: // Go back
                        case CMD.BACK:
                            Bookmark = Page.OPEN;
                            break;
                        case CMD.FORW: // Email acceptable
                            Bookmark = Page.LOGIN_P;
                            break;
                        case CMD.RETRY: // Input error
                            continue;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;

                case Page.LOGIN_P:
                    LOGIN_P_Prompt();
                    Orders = LOGIN_P_ProcessInput(out GivenPassword);
                    switch (Orders)
                    {
                        case CMD.OPT9: // Go back
                        case CMD.BACK:
                            Bookmark = Page.OPEN;
                            continue;
                        case CMD.FORW: // Password acceptable
                            Bookmark = Page.LOGIN_V;
                            break;
                        case CMD.FAIL: // Something about the password is bad
                            Bookmark = Page.LOGIN_E;
                            break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            break;
                    }
                    break;

                case Page.LOGIN_V:
                    Orders = LOGIN_V_ValidateLogin(GivenUsername, GivenPassword);
                    switch (Orders)
                    {
                        case CMD.FORW: // Login successful
                            //Bookmark = Page.WELCOME;
                            break;
                        case CMD.FAIL: // Credentials not found
                            Bookmark = Page.LOGIN_E;
                            break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                    }
                    break;
                default:
                    PageDoesNotExist(Bookmark);
                    return;
            }
        }
    }


    ///// Shared Validation Methods /////
    private int CheckForValidNumerical(string input, int[] valids)
    {
        int userInputNum;
        //if (string.IsNullOrEmpty(input)) return -1;
        //else 
        if (Int32.TryParse(input, out userInputNum))
        {
            if (valids.Contains(userInputNum)) return userInputNum;
            else return -1;
        }
        return -1;
    }
    private bool ValidateEmailFormat(string userString)
    {
        int theATIndex, theATIndex2;
        // find the email paradigm, then check for invalid characters
        if ((theATIndex = userString.IndexOf("@revature.com")) < 0) // find email paradigm
        {
            if (((theATIndex2 = userString.IndexOf("@")) != theATIndex)
                || ((theATIndex2 = userString.IndexOf("@", theATIndex + 1)) > 0)
                || !CheckNoEmailBannedChars(userString))
                return false;
            return true;
        }
        else if
            ((theATIndex = userString.IndexOf("@revature.net")) < 0) // find email paradigm
        {
            // check for invalid characters
            if (((theATIndex2 = userString.IndexOf("@")) != theATIndex)
                || ((theATIndex2 = userString.IndexOf("@", theATIndex + 1)) > 0)
                || !CheckNoEmailBannedChars(userString))
                return false;
            return true;
        }
        return false;
    }
    private bool CheckNoEmailBannedChars(string email)
    {
        return true;
    }
    private bool ValidatePasswordFormat(string password)
    {
        //if (password.Length >= 8)
        return true;
        return false;
    }


    ///// Pages and Processing /////
    /// shared and common strings
    private readonly string NineToGoHome = "You can press 9 and 'enter' to go back to the homepage instead.";

    private void OPEN_WelcomeText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Welcome to ERS Ticketing.");
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter').");
        sb.AppendLine("1.Login");
        sb.AppendLine("2.Sign-up");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private CMD OPEN_ProcessInput()
    {
        IInput consoleBased = new ConsoleBased();

        // Fetch the user's input string and ensure it is a 1 or 2
        string userInputMessage = consoleBased.GetUserInput();
        int userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 1, 2 });
        if (userInputNum > 0) return (CMD)userInputNum;
        consoleBased.DisplayPage(new string[] { "Please enter a number from the selection." });
        return CMD.RETRY;
    }

    private void SIGNU_E_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to create your account");
        sb.AppendLine("Please type in your full email (eg: name@demo.com) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private CMD SIGNU_A_CheckEmailAvailability(string email)
    {
        return CMD.NIL;
    }
    private void SIGNU_P_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to create your account");
        sb.AppendLine("Please type in your password (eg: password) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private CMD SIGNU_V_ValidateLogin(string username, string password)
    {
        return CMD.NIL;
    }

    private void LOGIN_E_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to log you in");
        sb.AppendLine("Please type in your full email (eg: name@demo.com) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private CMD LOGIN_E_ProcessInput(out string email)
    {
        email = "";
        string userInputMessage;
        int userInputNum;
        IInput cB = new ConsoleBased();

        // Fetch the user's input string
        userInputMessage = cB.GetUserInput();

        // check for 9
        userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 9 });
        if (userInputNum == 9) return CMD.BACK;

        // check for the email fomat xxx@revature.com/net
        // compare input to expected email paradigms
        /*        if (!ValidateEmailFormat(userInputMessage))
                {
                    cB.DisplayPage(new string[] { "The input was not recognized as an email.");
                    return CMD.RETRY;
        MVP        }*/

        // must be an email if its made it here
        email = userInputMessage;
        return CMD.FORW;
    }
    private void LOGIN_P_Prompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Preparing to log you in");
        sb.AppendLine("Please type in your password (eg: password) and press 'enter'.");
        sb.AppendLine(NineToGoHome);
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private CMD LOGIN_P_ProcessInput(out string password)
    {
        password = "";
        string userInputMessage;
        int userInputNum;
        IInput consoleBased = new ConsoleBased();

        // Fetch the user's input string
        userInputMessage = consoleBased.GetUserInput();

        // check for 9
        userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 9 });
        if (userInputNum == 9) return CMD.BACK;

        // check the password conformity rules
        if (!ValidatePasswordFormat(userInputMessage))
        {
            consoleBased.DisplayPage(new string[] { "This password cannot be accepted." });
            return CMD.FAIL;
        }

        // the password must be fine if its made it here
        password = userInputMessage;
        return CMD.FORW;
    }
    private CMD LOGIN_V_ValidateLogin(string username, string password)
    {
        return CMD.NIL;
    }

    private void LANDING_Manager_WelcomeText(User Dave)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Welcome back {Dave.userName}.");

        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Check tickets");
        sb.AppendLine("2.Promote employees");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }
    private void LANDING_Manager_ViewTickets(User Dave/*, filter method*/)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Now displaying reimbursments:");
        // sb.AppendLine(call function that gets all the approved tickets)
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Find a subset of tickets");
        sb.AppendLine("2.Approve pending tickets");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }

    private void LANDING_Employee_WelcomeText(User Dave)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Welcome {Dave.userName}.");
        sb.AppendLine("Now displaying reimbursments:");
        // sb.AppendLine(call function that gets all the approved tickets)
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Find a subset of tickets");
        sb.AppendLine("2.Approve pending tickets");
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { sb.ToString() });
    }

    private void PageDoesNotExist(Page which)
    {
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { "The requested page does not exist: " + which });
    }

    private void NESTED_DEFAULT_ERROR(Page where, CMD why)
    {
        IInput page = new ConsoleBased();
        page.DisplayPage(new string[] { $"Error on Page {where} due to Command {why}.\nExiting Program" });
    }

    ///// Database Interaction /////
    private bool CheckForExistingUser(string path, string email)
    {
        if (email != null)
        {
            return true;
        }
        return false;
    }

    private CMD CreateLoginCredentials(string path, string email, string password)
    {
        // check user does not already exist
        if (CheckForExistingUser(path, email)) return CMD.FAIL;

        // check email format
        if (email == "") return CMD.RETRY;
        return CMD.NIL;
        // create user

    }

    private CMD ConfirmLoginCredentials(string path, string email, string password)
    {
        if (email == "@2") // check email
        {
            if (password == "@2") // check password
            {
                return CMD.FORW; // pair accepted
            }
        }
        return CMD.FAIL;
    }


    // prompt 1.login or 2.signup
    //      1.login - 1.prompt email
    //          10.email - 1.prompt password
    //              10.pass - (logs in)
    //                  11.!pass - (increment counter), ask for retry or prompt go back
    //              11.!email - ask for retry or prompt go back
    //          2. prompt go back
    //      2.signup - 1.prompt email
    //          10.email - check email availability
    //              available - prompt password
    //              not - ask for retry or prompt go back
}
