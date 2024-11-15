using ClientMSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ClientMSystem.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<ClientDetail> clientDetails { get; set; }
        public DbSet<SignUp> signUps { get; set; }
        public DbSet<TimeSheet> timeSheets { get; set; }

        public DbSet <AdminModel> adminModel { get; set; }

        public DbSet<EmailDto> emailDto { get; set; }

    }
}
