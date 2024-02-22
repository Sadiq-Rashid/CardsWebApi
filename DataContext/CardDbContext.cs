using Cards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cards.Data
{
    public class CardDbContext : IdentityDbContext<IdentityUser>
    {
        public CardDbContext(DbContextOptions<CardDbContext>options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Card> Cards { get; set; }
       

    }
}
