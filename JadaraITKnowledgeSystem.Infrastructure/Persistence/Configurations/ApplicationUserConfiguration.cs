using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
public void Configure(EntityTypeBuilder<ApplicationUser> builder)
{
    builder.ToTable("ApplicationUsers");

    builder.Property(x => x.DomainUserId)
        .IsRequired();

    builder.Property(x => x.FullName)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(x => x.DateJoined)
        .IsRequired();

    // FK using the navigation property
    builder.HasOne(au => au.DomainUser)
            .WithOne() // no navigation from User side
            .HasForeignKey<ApplicationUser>(au => au.DomainUserId)
            .OnDelete(DeleteBehavior.Restrict); // avoid multiple cascade paths  // choose cascade/no action
    }
}