using Npgsql;
using System;

Console.WriteLine("--- Testing PostgreSQL Connection ---");
var connString = "Host=localhost;Port=5432;Database=VeXeDb;Username=postgres;Password=quang1501";
try {
    using var conn = new NpgsqlConnection(connString);
    conn.Open();
    Console.WriteLine("SUCCESS: Connection opened.");
    
    using var cmd = new NpgsqlCommand("SELECT count(*) FROM trips", conn);
    var count = cmd.ExecuteScalar();
    Console.WriteLine($"SUCCESS: Found {count} trips in 'trips' table.");
} catch (Exception ex) {
    Console.WriteLine($"FAILURE: {ex.Message}");
    
    // Try lowercase if uppercase fails or vice-versa
    try {
        using var conn2 = new NpgsqlConnection(connString);
        conn2.Open();
        using var cmd2 = new NpgsqlCommand("SELECT count(*) FROM \"Trips\"", conn2);
        var count = cmd2.ExecuteScalar();
        Console.WriteLine($"NOTE: Found {count} trips using quoted \"Trips\".");
    } catch {
         Console.WriteLine("NOTE: Quoted \"Trips\" also failed.");
    }
}
