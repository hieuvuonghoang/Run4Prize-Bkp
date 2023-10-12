using Microsoft.EntityFrameworkCore;

namespace Run4Prize.Models.DBContexts.AppContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<AthleteEntity> Athletes { get; set; }
        public DbSet<AccessTokenEntity> Tokens { get; set; }
        public DbSet<ActivityEntity> Activities { get; set; }
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<JobParameter> JobParameters { get; set; }
        public DbSet<WeekEntity> Weeks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeekEntity>()
                .ToTable("Weeks");
            modelBuilder.Entity<WeekEntity>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<WeekEntity>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<JobParameter>()
                .ToTable("JobParameters");
            modelBuilder.Entity<JobParameter>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<JobParameter>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LogEntity>()
                .ToTable("Logs");
            modelBuilder.Entity<LogEntity>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<LogEntity>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<AthleteEntity>()
                .ToTable("Athletes");
            modelBuilder.Entity<AthleteEntity>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<AthleteEntity>()
                .Property(it => it.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<AccessTokenEntity>()
                .ToTable("Tokens");
            modelBuilder.Entity<AccessTokenEntity>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<AccessTokenEntity>()
                .Property(it => it.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ActivityEntity>()
                .ToTable("Activities");
            modelBuilder.Entity<ActivityEntity>()
                .HasKey(it => it.Id);
            modelBuilder.Entity<ActivityEntity>()
                .Property(it => it.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<AthleteEntity>()
                .HasOne<AccessTokenEntity>(it => it.AccessToken)
                .WithOne(it => it.Athlete)
                .HasForeignKey<AccessTokenEntity>(it => it.AthleteId);

            modelBuilder.Entity<ActivityEntity>()
                .HasOne<AthleteEntity>(s => s.Athlete)
                .WithMany(g => g.Activities)
                .HasForeignKey(s => s.AthleteId);
        }
    }
}
