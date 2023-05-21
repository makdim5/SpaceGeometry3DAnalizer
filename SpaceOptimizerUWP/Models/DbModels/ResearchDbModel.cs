using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models.DbModels
{
    public class ResearchDbModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string FilePath { get; set; }

        public BaseResearch Research { get; set; }

        public List<Area> Areas { get; set; }
    }
}
