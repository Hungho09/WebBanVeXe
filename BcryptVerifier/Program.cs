using BCrypt.Net;
using System;

class Program
{
    static void Main()
    {
        string password = "Admin@123";
        string hash = "$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6";
        try {
            bool result = BCrypt.Net.BCrypt.Verify(password, hash);
            Console.WriteLine($"Verify Admin@123: {result}");
            
            string newHash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine($"New hash: {newHash}");
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
