using System;
using System.Collections.Generic;

namespace Userzone;

public class User : IEquatable<User>
{
    // Sub objects
    public enum Role { SimpleUser = 0, Employee, Manager };//,Admin };

    // Fields
    public string userName { get; set; }
    public string password { get; set; }
    public int UserId { get; set; }
    public Role EmployeeType { get; set; }

    // Constructor
    // This overload is for recreating the user returned from the database
    public User(string name, string pass, int Id, Role eType)
    {
        userName = name;
        password = pass;
        UserId = Id;
        EmployeeType =  eType;
    }

    // This overload is used to log the user in
    public User(string name, string pass)
    {
        userName = name;
        password = pass;
    }

    // Methods
    public override string ToString()
    {
        return "Username: " + userName + ". Role: " + EmployeeType.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        User objAsUser = obj as User;
        if (objAsUser == null) return false;
        else return Equals(objAsUser);
    }
    public override int GetHashCode()
    {
        return UserId;
    }

    /// <summary>
    /// Overrides object.Equals() and the == and != operators.
    /// Use to confirm correct login credentials
    /// </summary>
    /// <param name="other"></param>
    /// <returns>True if login credentials match</returns>
    public bool Equals(User other)
    {
        if (other == null) return false;
        return (this.userName.Equals(other.userName) && this.password.Equals(other.password));
    }
    
}

public class Example
{
    public static void Main()
    {
    }
}
/*        // Create a list of Users.
        List<User> userL = new List<User>();

        // Add Users to the list.
        userL.Add(new User("crank arm",   "", 1234, 0));
        userL.Add(new User("chain ring",  "", 1334, 0));
        userL.Add(new User("regular seat","", 1434, 0));
        userL.Add(new User("banana seat", "", 1444, 0));
        userL.Add(new User("cassette",    "", 1534, 0));
        userL.Add(new User("shift lever", "", 1634, 0));
        userL.Add(new User("name","pass", 0));

        // Write out the Users in the list. This will call the overridden ToString method
        // in the User class.
        Console.WriteLine();
        foreach (User aPart in userL)
        {
            Console.WriteLine(aPart);
        }

        // Check the list for part #0. This calls the IEquatable.Equals method
        // of the User class, which checks the UserId for equality.
        Console.WriteLine("\nContains: User with Id=0: {0}",
            userL.Contains(new User( "name","", 0 )));

        // Find items where name contains "seat".
        Console.WriteLine("\nFind: User where name contains \"seat\": {0}",
            userL.Find(x => x.userName.Contains("seat")));

        // Check if an item with Id 0 exists.
        Console.WriteLine("\nExists: User with Id=0: {0}",
            userL.Exists(x => x.UserId == 0));

        Console.WriteLine("\nContains: User with Id=0: {0}",
            userL.Contains(new User( "name","", 0 )));

        /*This code example produces the following output:

        ID: 1234   Name: crank arm
        ID: 1334   Name: chain ring
        ID: 1434   Name: regular seat
        ID: 1444   Name: banana seat
        ID: 1534   Name: cassette
        ID: 1634   Name: shift lever

        Contains: User with Id=1734: False

        Find: User where name contains "seat": ID: 1434   Name: regular seat

        Exists: User with Id=1444: True
        */
//    }
//}
