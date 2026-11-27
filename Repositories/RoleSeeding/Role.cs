using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.RoleSeeding
{
    public class Role:IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}
