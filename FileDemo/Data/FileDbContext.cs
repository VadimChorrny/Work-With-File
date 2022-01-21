using FileDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace FileDemo.Data
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) 
        { Database.EnsureCreated(); }
        public DbSet<File> Files { get; set; }
    }
}
