using Microsoft.EntityFrameworkCore;


namespace SpaceOptimizerUWP.Models.DbModels
{
    public class ResearchDbContext : DbContext
    {
        public DbSet<AdjacmentResearchDbModel> AdjacmentResearchs { get; set; }
        public DbSet<DbScanResearchDbModel> DbScanResearchs { get; set; }
        public DbSet<Area> Areas { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<Element> Elements { get; set; }

        public DbSet<Point3D> Points { get; set; }

        public ResearchDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Researches.db");
        }
    }
}
