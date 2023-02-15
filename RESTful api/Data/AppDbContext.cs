using Microsoft.EntityFrameworkCore;
using RESTful_api.Models;

namespace RESTful_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {

    }

    public DbSet<Book> Books { get; set; }

}
