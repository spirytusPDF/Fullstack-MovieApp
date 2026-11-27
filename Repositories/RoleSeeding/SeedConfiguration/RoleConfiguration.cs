using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.RoleSeeding.SeedConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("81AA91F1-4F6F-4E02-AC03-E0F64706C1D2"),
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "81AA91F1-4F6F-4E02-AC03-E0F64706C1D2",
                    Description = "Role for costumers and users without the ability to affect data"
                },
                new Role
                {
                    Id = Guid.Parse("0AF665C5-ED06-4E2C-8440-AD5CBC8E104F"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "0AF665C5-ED06-4E2C-8440-AD5CBC8E104F",
                    Description = "Role for admins who have full access to data"
                },
                new Role
                {
                    Id = Guid.Parse("A0A6E690-6CFB-47D8-A259-A86EDA667C16"),
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    ConcurrencyStamp = "A0A6E690-6CFB-47D8-A259-A86EDA667C16",
                    Description = "Role for managers who can affect data only with permission"
                }
            );
        }
    }
}
