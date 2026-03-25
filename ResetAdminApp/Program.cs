using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities;
using BCrypt.Net;

var connectionString = "Server=(localdb)\\mssqllocaldb;Database=WebBanVeXeDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using var db = new ApplicationDbContext(optionsBuilder.Options);

try {
    Console.WriteLine("--- Finalizing WebBanVeXeDB ---");
    var admin = db.Users.FirstOrDefault(u => u.UserName == "admin");
    if (admin != null) {
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        db.SaveChanges();
        Console.WriteLine("[v] Admin password set to Admin@123 in WebBanVeXeDB");
    } else {
        Console.WriteLine("[!] Admin user NOT FOUND? This is strange.");
    }
} catch (Exception ex) {
    Console.WriteLine($"[!] Error: {ex.Message}");
}
