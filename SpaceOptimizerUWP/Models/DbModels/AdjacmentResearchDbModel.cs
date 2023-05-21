using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models.DbModels
{
    public class AdjacmentResearchDbModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string FilePath { get; set; }

        public AdjacmentResearch Research { get; set; }

        public List<Area> Areas { get; set; }
    }
}
