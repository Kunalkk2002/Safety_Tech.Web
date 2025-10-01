using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Safety_Tech.Models.Models;

public partial class ApplicationDataContext : IdentityDbContext<IdentityUser>   //DbContext // :IdentityDbContext<User>  //: DbContext
{
  
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options)
        : base(options)
    {

    }

    public virtual DbSet<PayPeriod> PayPeriods { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectTeam> ProjectTeams { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<UserRole> UserRole { get; set; }
    
    public virtual DbSet<WorkTimeEntry> WorkTimeEntries { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    public virtual DbSet<Incidents> Objectdetections { get; set; }

    public virtual DbSet<ApprovedIncident> ApprovedIncidents { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<PayPeriod>(entity =>
        {
            entity.ToTable("PayPeriod");
        });


        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<ApprovedIncident>()
           .HasOne(a => a.Approver)              // ApprovedIncident has one User
           .WithMany()                       // A user can approve many incidents (if you want)
           .HasForeignKey(a => a.ApproveBy)  // FK property
           .HasPrincipalKey(u => u.Id);      // PK in AspNetUsers


        modelBuilder.Entity<UserRole>()
            .HasNoKey()
            .ToView(null); // Use ToView(null) to avoid creating a view or table





        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.HasIndex(e => e.UserId, "IX_Payment_UserId");

            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.ToDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Payments).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasIndex(e => e.ManagerId, "IX_PaymentStatuses_ManagerId");

            entity.HasIndex(e => e.PaymentId, "IX_PaymentStatuses_PaymentId");

            entity.HasIndex(e => e.ProjectId, "IX_PaymentStatuses_ProjectId");

            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Manager).WithMany(p => p.PaymentStatuses).HasForeignKey(d => d.ManagerId);

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentStatuses).HasForeignKey(d => d.PaymentId);

            entity.HasOne(d => d.Project).WithMany(p => p.PaymentStatuses).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");
        });

        modelBuilder.Entity<ProjectTeam>(entity =>
        {
            entity.ToTable("ProjectTeam");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectTeam_ProjectId");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTeams).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRole");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserRole_RoleId");
                    });
        });

        modelBuilder.Entity<WorkTimeEntry>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_WorkTimeEntries_ProjectId");

            entity.HasIndex(e => e.UserId, "IX_WorkTimeEntries_UserId");

            entity.HasIndex(e => e.WorkTypeId, "IX_WorkTimeEntries_WorkTypeId");

            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasDefaultValueSql("((0))");
            entity.Property(e => e.WorkDate).HasColumnType("datetime");
            entity.Property(e => e.WorkingHours).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.WorkTimeEntries).HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.User).WithMany(p => p.WorkTimeEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.WorkType).WithMany(p => p.WorkTimeEntries).HasForeignKey(d => d.WorkTypeId);
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.ToTable("WorkType");

            entity.Property(e => e.WorkTypeHours).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshToken");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Incidents>(entity =>
        {
            entity.ToTable("Objectdetection");
        });

        modelBuilder.Entity<ApprovedIncident>(entity =>
        {
            entity.ToTable("ApprovedIncident");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IncidentId).HasDatabaseName("IX_ApprovedIncident_IncidentId");
            entity.HasIndex(e => e.ApproveBy).HasDatabaseName("IX_ApprovedIncident_ApproveBy");

            entity.HasOne(e => e.Incident)
                .WithMany()
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<IdentityUser>(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApproveBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
