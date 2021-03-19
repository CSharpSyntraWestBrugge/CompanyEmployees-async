using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Security
{
    public class SecurityDbContext:IdentityDbContext
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options): base(options)
        {

        }

    }
}
