using Microsoft.EntityFrameworkCore;

namespace Kontakty.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext( DbContextOptions<DatabaseContext> options) : base(options) 
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
    }
}
