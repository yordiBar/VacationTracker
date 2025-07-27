using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Models;

namespace VacationTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Disable foreign key constraints for multi-tenant architecture
            builder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DepartmentName).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.DepartmentCode).HasMaxLength(20).HasColumnType("nvarchar(20)");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Firstname).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.Surname).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.DisplayName).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.EmployeeNumber).HasMaxLength(20).HasColumnType("nvarchar(20)");
                entity.Property(e => e.JobTitle).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.DepartmentId).HasColumnType("int");
                entity.Property(e => e.LocationId).HasColumnType("int");
                entity.Property(e => e.GenderId).HasColumnType("int");
                entity.Property(e => e.StartDate).HasColumnType("datetime2(7)");
                entity.Property(e => e.IsApprover).HasColumnType("bit");
                entity.Property(e => e.IsManager).HasColumnType("bit");
                entity.Property(e => e.IsAdmin).HasColumnType("bit");
                entity.Property(e => e.Mon).HasColumnType("bit");
                entity.Property(e => e.Tue).HasColumnType("bit");
                entity.Property(e => e.Wed).HasColumnType("bit");
                entity.Property(e => e.Thu).HasColumnType("bit");
                entity.Property(e => e.Fri).HasColumnType("bit");
                entity.Property(e => e.Sat).HasColumnType("bit");
                entity.Property(e => e.Sun).HasColumnType("bit");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<Gender>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LocationName).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.LocationCode).HasMaxLength(20).HasColumnType("nvarchar(20)");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<RequestType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestTypeName).HasMaxLength(100).HasColumnType("nvarchar(100)");
                entity.Property(e => e.RequesTypeCode).HasMaxLength(20).HasColumnType("nvarchar(20)");
                entity.Property(e => e.Description).HasMaxLength(500).HasColumnType("nvarchar(500)");
                entity.Property(e => e.TakesFromAllowance).HasColumnType("bit");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<Allowance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.From).HasColumnType("datetime2(7)");
                entity.Property(e => e.To).HasColumnType("datetime2(7)");
                entity.Property(e => e.Amount).HasColumnType("int");
                entity.Property(e => e.CarryOver).HasColumnType("int");
                entity.Property(e => e.EmployeeId).HasColumnType("int");
                entity.Property(e => e.IsDeleted).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
            builder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.From).HasColumnType("datetime2(7)");
                entity.Property(e => e.To).HasColumnType("datetime2(7)");
                entity.Property(e => e.RequestTypeId).HasColumnType("int");
                entity.Property(e => e.EmployeeId).HasColumnType("int");
                entity.Property(e => e.RequestAmount).HasColumnType("int");
                entity.Property(e => e.Description).HasMaxLength(500).HasColumnType("nvarchar(500)");
                entity.Property(e => e.Status).HasColumnType("int");
                entity.Property(e => e.RequestCreatedByEmployeeId).HasColumnType("int");
                entity.Property(e => e.IsActive).HasColumnType("bit");
                entity.Property(e => e.CompanyId).HasColumnType("int");
            });
        }
    }
}
