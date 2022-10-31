using System;

namespace ERS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ERS Ticketing");
            Console.WriteLine("Please make a selection (type the corresponding # and press 'enter'");
            Console.WriteLine("1.Login \n2.Sign-up");

            // prompt 1.login or 2.signup
                // 1.login - 1.prompt email
                    // 10.email - 1.prompt password
                        // 10.pass - (logs in)
                        // 11.!pass - (increment counter), ask for retry or prompt go back
                    // 11.!email - ask for retry or prompt go back
                    // 2. prompt go back
                // 2.signup - 1.prompt email
                    // 10.email - check email availability
                        // available - prompt password
                        // not - ask for retry or prompt go back
                
        }
    }
}