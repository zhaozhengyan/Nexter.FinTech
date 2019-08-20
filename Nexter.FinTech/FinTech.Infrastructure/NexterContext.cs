using FinTech.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure
{
    public class NexterContext : DbContext
    {
        public const string Ids = "Ids";
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public NexterContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Member>(eb =>
            {
                eb.Property(i => i.Id).ForSqlServerUseSequenceHiLo(Ids);
            });
            modelBuilder.Entity<Transaction>(eb =>
            {
                eb.Property(i => i.Id).ForSqlServerUseSequenceHiLo(Ids);
                eb.Property(x => x.Spending).HasColumnType("decimal(18, 4)");
                eb.Property(x => x.Income).HasColumnType("decimal(18, 4)");
            });
            modelBuilder.Entity<Account>(eb =>
            {
                eb.Property(i => i.Id).ForSqlServerUseSequenceHiLo(Ids);
                eb.Property(x => x.Type).HasConversion<string>();
            });
            modelBuilder.Entity<Category>(eb =>
            {
                eb.Property(i => i.Id).ForSqlServerUseSequenceHiLo(Ids);
                eb.Property(x => x.Type).HasConversion<string>();
            });
            modelBuilder.Entity<Group>(eb =>
            {
                eb.Property(i => i.Id).ForSqlServerUseSequenceHiLo(Ids);
            });
        }
    }
}


