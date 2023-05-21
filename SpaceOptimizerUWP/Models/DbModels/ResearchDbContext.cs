using Microsoft.EntityFrameworkCore;


namespace SpaceOptimizerUWP.Models.DbModels
{
    public class ResearchDbContext : DbContext
    {
        public DbSet<AdjacmentResearchDbModel> AdjacmentResearchs { get; set; }
        public DbSet<DbScanResearchDbModel> DbScanResearchs { get; set; }

        public ResearchDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=myresearches.db");
        }
    }
}
