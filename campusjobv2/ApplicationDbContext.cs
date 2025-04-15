using campusjobv2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace campusjobv2
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<StudentWorker> StudentWorkers { get; set; }
        public DbSet<RightToWorkDocument> RightToWorkDocuments { get; set; }
        public DbSet<VisaStatus> VisaStatuses { get; set; }
        public DbSet<OfferedShift> OfferedShifts { get; set; }
        
        // public DbSet<ApprovedShift> ApprovedShifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.User_ID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Recruiter)
                .WithOne(r => r.User)
                .HasForeignKey<Recruiter>(r => r.User_ID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.User_ID);
                
            modelBuilder.Entity<Recruiter>()
                .HasMany(r => r.Employees)
                .WithOne(e => e.Recruiter)
                .HasForeignKey(e => e.Recruitment_ID);
                
            modelBuilder.Entity<Recruiter>()
                .HasMany(r => r.StudentWorkers)
                .WithOne(sw => sw.Recruiter)
                .HasForeignKey(sw => sw.Recruitment_ID);
                
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.RightToWorkDocuments)
                .WithOne(rtw => rtw.Employee)
                .HasForeignKey(rtw => rtw.Student_ID);
                
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.VisaStatuses)
                .WithOne(vs => vs.Employee)
                .HasForeignKey(vs => vs.Student_ID);
                
            modelBuilder.Entity<OfferedShift>()
                .HasOne(os => os.Employee)
                .WithMany(e => e.OfferedShifts)
                .HasForeignKey(os => os.Student_ID);
                
            modelBuilder.Entity<OfferedShift>()
                .HasOne(os => os.Recruiter)
                .WithMany(r => r.OfferedShifts)
                .HasForeignKey(os => os.Recruitment_ID);
                
      
            // modelBuilder.Entity<OfferedShift>()
            //     .HasMany(os => os.ApprovedShifts)
            //     .WithOne(ash => ash.OfferedShift)
            //     .HasForeignKey(ash => ash.Offer_ID);
        }
    }
}
