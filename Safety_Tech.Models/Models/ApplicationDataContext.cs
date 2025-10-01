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

    public virtual DbSet<Incidents> Incidents { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

       

        base.OnModelCreating(modelBuilder);



        modelBuilder.Entity<Incidents>(entity =>
        {
            entity.ToTable("Incidents");
        });

      

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
