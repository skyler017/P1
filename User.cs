using System.Text; // String builder?
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UserAccess
{
    [Serializable()]
    public class User
    {
        // Fields
        string userName;
        [field: NonSerialized()] string password;


        // Costructors
        public User (){}

        public User(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        // Methods
        public string DisplayRecord(string path)
        {
            string[] records = File.ReadAllLines(path);
            StringBuilder result = new StringBuilder();
            
            result.AppendLine("Username \t\t User type");
            foreach (string record in records)
            {   
                string[] current = record.Split(", ");
                
                result.AppendLine($"{current[0]} \t{current[2]}");

//User current = new User(current[0], current[1]);
                //result.AppendLine(record);
                //result.AppendLine($"{userName} \t{this.wins}\t\t {this.losses}\t\t {this.averageTurns}");
            }
            return result.ToString();

            // string result = "this string";
            // result = result + "some other string being concatenated";
            // return result;

            //return "this string";
        }

        public void SaveRecord(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.userName}, {this.password}");
            File.AppendAllText(path, sb.ToString());
        }     
    }

}