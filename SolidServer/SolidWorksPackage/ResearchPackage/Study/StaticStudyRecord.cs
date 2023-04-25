using System.Collections.Generic;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class StaticStudyRecord
    {
        public readonly string text = "StaticStudy";

        public readonly Material material;

        public readonly IEnumerable<FeatureFace> fixFaces;

        public readonly IEnumerable<FeatureFace> loadFaces;

        public readonly Mesh mesh;

        public readonly int index;

        public StaticStudyRecord(
            int index,
            Material material, 
            IEnumerable<FeatureFace> fixFaces,
            IEnumerable<FeatureFace> loadFaces, 
            Mesh mesh) 
        {
            this.index = index;
            this.material = material;
            this.fixFaces = fixFaces;
            this.loadFaces = loadFaces;
            this.mesh = mesh;
        }

        public override string ToString()
        {
            return $"Исследование №{index}: {text} Материал:{material}";
        }

    }
}
