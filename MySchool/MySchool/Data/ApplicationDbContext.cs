using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySchool.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySchool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<PermissionSlip> PermissionSlips { get; set; }
        public DbSet<EmergencyCard> EmergencyCards { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<SchedulerEvent> SchedulerEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole
                    {
                        Id = "7054f81b-74af-470c-90a3-d83cadef5894",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = "b575f2c7-3016-4b8a-9878-3261ed0ffc25"
                    },
                    new IdentityRole
                    {
                        Id = "3dc8a3d0-45ae-4623-8435-39c8ee827a3d",
                        Name = "Teacher",
                        NormalizedName = "TEACHER",
                        ConcurrencyStamp = "e6f1923c-dd5e-4da2-8b1a-a58fc8cb8d9c"
                    },
                    new IdentityRole
                    {
                        Id = "71d45ad9-feb4-4f4f-a78b-5fa46f52da3e",
                        Name = "Parent",
                        NormalizedName = "PARENT",
                        ConcurrencyStamp = "2bd4edbd-a0f9-4172-a059-7dce5693c5f4"
                    }
                );
        }
    }
}
