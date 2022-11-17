using System;
using System.Collections.Generic;

namespace RSTS.Userzone;

public class User : IEquatable<User>
{
    // Sub objects
    public enum Role { SimpleUser = 0, Employee, Manager };//,Admin };

    // Fields
    public int UserID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Role EmployeeType { get; set; }

    // Constructors
    /// <summary>
    /// This overload is the empty User object that gets filled by the database.
    /// </summary>
    public User() { }

    /// <summary>
    /// This overload is the complete User object returned by the database
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pass"></param>
    /// <param name="Id"></param>
    /// <param name="eType"></param>
    public User(string name, string pass, int Id, Role employeeType)
    {
        UserID = Id;
        Username = name;
        Password = pass;
        EmployeeType = employeeType;
    }
    
    /// <summary>
    /// This overload is the partial User object created by the user
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pass"></param>
    public User(string name, string pass)
    {
        Username = name;
        Password = pass;
    }

    // Methods
    public override string ToString()
    {
        return "Username: " + Username + ". Role: " + EmployeeType.ToString();
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
        return UserID;
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
        return (this.Username.Equals(other.Username) && this.Password.Equals(other.Password));
    }
    
}
