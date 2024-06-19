using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using School_Management_API.Models;
using System.Reflection.Emit;

namespace School_Management_API.Data
{
    public class SchoolDbContext :IdentityDbContext<AppUser>
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options):base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //admin and user
            builder.Entity<AppUser>()
              .HasOne(u => u.Admin) // AppUser has one Admin
              .WithOne(a => a.User) // Admin has one AppUser (inverse side)
              .HasForeignKey<Admin>(a => a.AppUserId) // Foreign key in Admin table
               .OnDelete(DeleteBehavior.Cascade);

            //teacher and user
            builder.Entity<AppUser>()
              .HasOne(u => u.Teacher) // AppUser has one Admin
              .WithOne(a => a.User) // Admin has one AppUser (inverse side)
              .HasForeignKey<Teacher>(a => a.AppUserId) // Foreign key in Admin table
               .OnDelete(DeleteBehavior.Cascade);



            //Student and user
            builder.Entity<AppUser>()
              .HasOne(u => u.Student) // AppUser has one Admin
              .WithOne(a => a.User) // Admin has one AppUser (inverse side)
              .HasForeignKey<Student>(a => a.AppUserId) // Foreign key in Admin table
              .OnDelete(DeleteBehavior.Cascade);



            // Configure one-to-many relationship between Class and Subject
            builder.Entity<Subject>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);



            // Configure one-to-many relationship between Teacher and Subject
            builder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);



            // Configure many-to-one relationship between Student and Class
            builder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Call the SeedRoles method to seed roles
            SeedRoles(builder);

        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() { Name = "Teacher", ConcurrencyStamp = "2", NormalizedName = "TEACHER" },
                new IdentityRole() { Name = "Student", ConcurrencyStamp = "3", NormalizedName = "STUDENT" },
                new IdentityRole() { Name = "Accountant", ConcurrencyStamp = "4", NormalizedName = "ACCOUNTANT" },
                new IdentityRole() { Name = "Parent", ConcurrencyStamp = "5", NormalizedName = "PARENT" }

                );
        }



        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }




        //public DbSet<Parent> Parents { get; set; }
    }
}
