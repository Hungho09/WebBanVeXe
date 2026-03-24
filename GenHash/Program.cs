using BCrypt.Net;
using System;

string password = "Admin@123";
string hash = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine(hash);
