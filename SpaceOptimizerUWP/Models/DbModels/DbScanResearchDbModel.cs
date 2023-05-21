using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models.DbModels
{
    public class DbScanResearchDbModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string FilePath { get; set; }

        public DbscanResearch Research { get; set; }

        public List<Area> Areas { get; set; }
    }
}
