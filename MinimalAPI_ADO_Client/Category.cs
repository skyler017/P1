using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAPI_ADO_Client
{
    public class Category
    {
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return "ID: " + Categoryid + " Name: " + CategoryName + " Desc: " + Description;
        }
    }
}
