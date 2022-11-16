using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface;

public interface IInput
{
    // Contract Methods
    public string GetUserInput();
    public void DisplayPage(string[] words);
}
