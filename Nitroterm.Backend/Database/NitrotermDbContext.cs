using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Database;

public class NitrotermDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Post> Posts { get; set; }
    
    public NitrotermDbContext()
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!File.Exists("connection_string.txt"))
        {
            throw new Exception(
                "No connection string file. Please create a connection_string.txt file next to the program " +
                "in order to be able to connect to the database");
        }
        
        optionsBuilder.UseMySQL(
            new MySqlConnection(File.ReadAllText("connection_string.txt")));
    }
}