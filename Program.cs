using System;
using System.IO;
using UserAccess;

namespace ERS
{
    class Program
    {
        private enum Page { STARTUP=0, WELCOME
            ,LOGIN_E,LOGIN_P//,LOGIN_EM,LOGIN_PM
            ,SIGN_E,SIGN_EA,SIGN_P//,SIGN_EM,SIGN_EAM,SIGN_PM
            }
        private enum CMD { NIL=-1
                //, OPT0, OPT1, OPT2 /**/, OPT9
                , PASS, FAIL, RETRY
        };

        const string Path = "./.LocalUserList";


        public void StateMachine()
        {
            Page Bookmark = Page.STARTUP;
            CMD Orders = CMD.NIL;
            string userInputMessage;
            int userInput = -1;
            User Dave;

            
            while (1 == 1)
            {
                switch (Bookmark)
                {
                    case Page.STARTUP:
                        Console.WriteLine("Welcome to ERS Ticketing");
                        Console.WriteLine("Please make a selection (type the corresponding # and press 'enter'");
                        Console.WriteLine("1.Login \n2.Sign-up");
                        userInputMessage = Console.ReadLine();
                        if (!Int32.TryParse(userInputMessage, out userInput) || userInput <= 0)
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
                            Orders = checkForEmail(userInputMessage);
                            switch (Orders)
                            {
                                case CMD.PASS: // email found
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
                        Console.WriteLine("The requested page does not exist: " + Bookmark);
                        break;
                }
            }
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
            if (email == null) return CMD.RETRY;

            // create user
            
        }

        private CMD ConfirmLoginCredentials(string path, string email, string password)
        {
            if (email == "@2") // check email
            {
                if (password == "@2") // check password
                {
                    return CMD.PASS; // pair accepted
                }
            }
            return CMD.FAIL;
        }

        private CMD checkForEmail(string input)
        {
            if (input[0] == '@')
            {
                if (input == "@2")
                {
                    Console.WriteLine("And next..."); return CMD.PASS;
                    // now to check pass
                }
                else
                {
                    // email not found
                    Console.WriteLine("We couldn't find your email in our system"); return CMD.FAIL;
                    // maybe they typed it wrong or they still need to put it in the system
                    //Bookmark = Page.SIGN_EM;                }
                }
            }
            else
            {
                // input was not in an expected format

                Console.WriteLine("Unrecognized text input. Please use your full name@demo.com email");
                //return Cmd.RETRY;
                return CMD.FAIL;
            }
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
}