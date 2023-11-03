using Microsoft.EntityFrameworkCore;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Distance> Distances { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .ToTable("Teams");
            modelBuilder.Entity<Team>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<Team>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Team>()
                .Property(it => it.Name)
                .HasMaxLength(50)
                .IsRequired(true);

            modelBuilder.Entity<Member>()
                .ToTable("Members");
            modelBuilder.Entity<Member>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<Member>()
                .HasIndex(it => it.TeamId);
            modelBuilder.Entity<Member>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Member>()
                .Property(it => it.Name)
                .HasMaxLength(50)
                .IsRequired(true);

            modelBuilder.Entity<Distance>()
                .ToTable("Distances");
            modelBuilder.Entity<Distance>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<Distance>()
                .HasIndex(it => it.MemberId);
            modelBuilder.Entity<Distance>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Distance>()
                .Property(it => it.CreateDate)
                .IsRequired(true);
            modelBuilder.Entity<Distance>()
                .Property(it => it.TotalDistance)
                .IsRequired(true);

            modelBuilder.Entity<Setting>()
                .ToTable("Settings");
            modelBuilder.Entity<Setting>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<Setting>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Setting>()
                .Property(it => it.Type)
                .IsRequired(true);
            modelBuilder.Entity<Setting>()
                .Property(it => it.Value)
                .HasMaxLength(500)
                .IsUnicode(true);

            modelBuilder.Entity<Log>()
                .ToTable("Logs");
            modelBuilder.Entity<Log>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<Log>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Log>()
                .Property(it => it.Type)
                .HasMaxLength(10)
                .IsRequired(true);
            modelBuilder.Entity<Log>()
                .Property(it => it.Mess)
                .IsRequired(true)
                .IsUnicode(true);
        }
    }
}
