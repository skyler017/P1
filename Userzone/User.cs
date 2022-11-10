using System;
using System.Collections.Generic;

namespace Userzone;

public class User : IEquatable<User>
{
    public int UserId { get; set; }
    public string userName { get; set; }
    private string password;
    public enum Role { SimpleUser = 0, Employee, Manager };//,Admin };
    public Role EmployeeType { get; set; }

    public User(int Id, string name, string pass = "pass", int eType = 0)
    {
        UserId = Id;
        userName = name;
        password = pass;
        EmployeeType = (Role) eType;
    }

    public override string ToString()
    {
        return "Name: " + userName;
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
        // Create a list of Users.
        List<User> userL = new List<User>();

        // Add Users to the list.
        userL.Add(new User(1234, "crank arm"));
        userL.Add(new User(1334, "chain ring"));
        userL.Add(new User(1434, "regular seat"));
        userL.Add(new User(1444, "banana seat"));
        userL.Add(new User(1534, "cassette"));
        userL.Add(new User(1634, "shift lever"));
        userL.Add(new User(0,"name","pass"));

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
            userL.Contains(new User( 0, "name" )));

        // Find items where name contains "seat".
        Console.WriteLine("\nFind: User where name contains \"seat\": {0}",
            userL.Find(x => x.userName.Contains("seat")));

        // Check if an item with Id 0 exists.
        Console.WriteLine("\nExists: User with Id=0: {0}",
            userL.Exists(x => x.UserId == 0));

        Console.WriteLine("\nContains: User with Id=0: {0}",
            userL.Contains(new User( 0, "name" )));

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
    }
}
