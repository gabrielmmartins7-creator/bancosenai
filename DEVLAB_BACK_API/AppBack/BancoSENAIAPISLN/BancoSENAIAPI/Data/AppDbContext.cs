using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace BancoSENAIAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 

        public DbSet<Agencia> Agencia => Set<Agencia>();
    }
}
