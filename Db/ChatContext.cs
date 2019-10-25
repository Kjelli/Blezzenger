using Blezzenger.Models;
using Microsoft.EntityFrameworkCore;

namespace Blezzenger.Db
{
    public class ChatContext : DbContext
    {
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blezzenger;Trusted_Connection=True;");
            }
        }
    }
}
