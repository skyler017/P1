﻿using System;
using Userzone;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/

namespace DataInfrustructure;

public interface IRepository
{
    // Contract Methods
    public string GetCredentials(string username, string password);
    public User FindUser(string username, string password);

}