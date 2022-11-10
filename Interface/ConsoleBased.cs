using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class ConsoleBased : IInput
    {
        // Fields
        

        // Constructor
        public ConsoleBased() { }

        public string GetUserInput()
        {
            string incoming = Console.ReadLine();
            if (incoming != null) return incoming;
            else return "";
        }
        public void DisplayPage(string page)
        {
            Console.WriteLine(page);
        }
    }
}
