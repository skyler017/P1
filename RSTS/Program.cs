using System;
using System.IO;
using Userzone;
using DataInfrustructure;
using Interface;
using System.Text;

namespace ERS;

class Program
{
    private enum Page { STARTUP=0, WELCOME
        ,LOGIN_E,LOGIN_P//,LOGIN_EM,LOGIN_PM
        ,SIGN_E,SIGN_EA,SIGN_P//,SIGN_EM,SIGN_EAM,SIGN_PM
        }
    private enum CMD { NIL=-1
            , OPT1=1, OPT2=2 /**/ //, OPT9, OPT0
            , FORW, BACK, FAIL, RETRY
    };

    const string Path = "./.LocalUserList";


    public void StateMachine()
    {
        Page Bookmark = Page.STARTUP;
        CMD Orders = CMD.NIL;
        string userInputMessage; //// Get rid of this
        int userInput; ///// Get rid of this
        User Dave;
        string GivenUsername = "";
        string GivenPassword = "";

        
        while (1 == 1)
        {
            switch (Bookmark)
            {
                case Page.STARTUP:
                    STARTUP_WelcomeText();
                    Orders = STARTUP_ProcessInputs();
                    switch (Orders)
                    {
                        case CMD.OPT1: // Existing User
                            Orders = CMD.NIL;
                            Bookmark = Page.LOGIN_E;
                            break;
                        case CMD.OPT2: // New User
                            Orders = CMD.NIL; 
                            Bookmark = Page.SIGN_E;
                            break;
                        case CMD.RETRY: // input error
                            Orders = CMD.NIL;
                            continue;
                            //break;
                        default:
                            NESTED_DEFAULT_ERROR(Bookmark, Orders);
                            return;
                            //break;
                    }
                    break;

                case Page.LOGIN_E:
                    Console.WriteLine("Preparing to log you in");
                    Console.WriteLine("Please type in your full email (eg: name@demo.com) and press 'enter'.");
                    Console.WriteLine("You can press 9 and 'enter' to go back to the homepage");
                    userInputMessage = Console.ReadLine();
                    if (Int32.TryParse(userInputMessage, out userInput))
                    {
                        if (userInput == 9) Bookmark = Page.STARTUP;
                        else
                        {
                            Console.WriteLine("Only the number 9 is an acceptable input");
                            continue;
                        }
                    }
                    else
                    {
                        // check that the input string is in the correct format xxx@revature.com/net
                        // compare input to working emails
                        Orders = checkForEmail(userInputMessage, GivenUsername);
                        switch (Orders)
                        {
                            case CMD.FORW: // email found
                                Orders = CMD.NIL;
                                Bookmark = Page.SIGN_P;
                                break;
                            case CMD.FAIL: // not in system or was input wrong
                                Orders = CMD.NIL;
                                //Bookmark = Page.SIGN_EM;
                                continue;
                                break;
                            default:
                                Console.WriteLine("Unhandled email return");
                                return;
                        }
                    }
                    break;
                case Page.SIGN_P:
                    Console.WriteLine("Please type in your password (eg: password) and press 'enter'.");
                    Console.WriteLine("You can type 9 and press 'enter' to go back to the homepage");
                    userInputMessage = Console.ReadLine();
                    if (Int32.TryParse(userInputMessage, out userInput))
                    {
                        if (userInput == 9) Bookmark = Page.STARTUP;
                        else
                        {
                            Console.WriteLine("Only the number 9 is an acceptable input");
                            continue;
                        }
                    }
                    break;
                default:
                    PageDoesNotExist(Bookmark);
                    return;
                    //break;
            }
        }
    }
    ///// general methods /////
    private int CheckForValidNumerical(string input, int[] valids)
    {
        int userInputNum;
        if (string.IsNullOrEmpty(input)) return -1;
        else if(Int32.TryParse(input, out userInputNum))
        {
            if (valids.Contains(userInputNum)) return userInputNum;
            else return -1;
        }
        return -1;
    }
    private string CheckForEmail(string input)
    {
        return "0";
    }

    private void NESTED_DEFAULT_ERROR(Page where, CMD why)
    {
        IInput page = new ConsoleBased();
        page.DisplayPage($"Error on Page {where} due to Command {why}.\nExiting Program");
    }

    ///// Pages and Processing /////
    private void STARTUP_WelcomeText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Welcome to ERS Ticketing.");
        sb.AppendLine("Please make a selection (type the corresponding # and press 'enter'.");
        sb.AppendLine("1.Login");
        sb.AppendLine("2.Sign-up");
        IInput page = new ConsoleBased();
        page.DisplayPage(sb.ToString());
    }

    private CMD STARTUP_ProcessInputs()
    {
        string userInputMessage;
        int userInputNum;
        ConsoleBased consoleBased = new ConsoleBased();

        // Fetch the user's input string and ensure it is a 1 or 2
        userInputMessage = consoleBased.GetUserInput();
        userInputNum = CheckForValidNumerical(userInputMessage, new int[] { 1, 2});
        if (userInputNum > 0) return (CMD)userInputNum;
        consoleBased.DisplayPage("Please enter a positive whole number");
        return CMD.RETRY;
/*        if (!Int32.TryParse(userInputMessage, out userInput) || userInput <= 0)
        {
            Console.WriteLine("Please enter a positive whole number");//: " + userInput);
            continue;
        }
        if (userInput == 1) Bookmark = Page.LOGIN_E;
        //else if (userInput == 2) Bookmark = Page.SIGN_E;
        else
        {
            Console.WriteLine("Please only input numbers from the selection");
            continue;
        }*/
        return CMD.NIL;
    }
    private void PageDoesNotExist(Page which)
    {
        Console.WriteLine("The requested page does not exist: " + which);
    }
    private bool CheckForExistingUser(string path, string email)
    {
        if(email != null)
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

    private CMD checkForEmail(string input, string? theEmail)
    {
        CMD output = CMD.NIL;
        if (input[0] == '@')
        {
            if (input == "@2")
            {
                //Console.WriteLine("And next...");
                output = CMD.FORW;
                // now to check pass
            }
            else
            {
                output = CMD.FORW;
                // email not found
                //Console.WriteLine("We couldn't find your email in our system"); return CMD.FAIL;
                // maybe they typed it wrong or they still need to put it in the system
                //Bookmark = Page.SIGN_EM;                }
            }
        }
        else
        {
            // input was not in an expected format
            //Console.WriteLine("Unrecognized text input. Please use your full name@demo.com email");
            theEmail = null;
            output = CMD.FAIL;
        }
        return output;
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

    static void Main(string[] args)
    {
        Program thread = new Program();
        thread.StateMachine();
        Console.WriteLine("Error: Program caused a return");
    }
}