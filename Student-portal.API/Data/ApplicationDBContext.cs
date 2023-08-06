using Microsoft.EntityFrameworkCore;
using Student_portal.API.Models;
using System.Collections.Generic;

namespace Student_portal.API.Data
{
    public class ApplicationDBContext :DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
    }
}
