using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.RoleSeeding.SeedConfiguration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("45dafa4b-7fc6-4286-4aa4-08df4359550c"),
                    RoleId = Guid.Parse("0AF665C5-ED06-4E2C-8440-AD5CBC8E104F")
                },
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("d0d47afa-af49-4d57-42b6-08df434c9869"),
                    RoleId = Guid.Parse("a0a6e690-6cfb-47d8-a259-a86eda667c16")
                }

            );
        }
    }
}
