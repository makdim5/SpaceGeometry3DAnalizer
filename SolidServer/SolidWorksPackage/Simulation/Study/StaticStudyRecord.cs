using SolidServer.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.SolidWorksPackage.Simulation.MaterialWorker;
using SolidServer.SolidWorksPackage.Simulation.MeshWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidServer.Simulation.Study
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
