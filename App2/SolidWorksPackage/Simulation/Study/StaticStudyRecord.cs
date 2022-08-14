using App2.SolidWorksPackage.Simulation.MaterialWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.Simulation.Study
{
    public class StaticStudyRecord
    {
        public readonly string text = "StaticStudy";

        public readonly Material material;

        public readonly IEnumerable<StudyFace> fixFaces;

        public readonly IEnumerable<StudyFace> loadFaces;

        public readonly StudyMesh mesh;

        public readonly int index;

        public StaticStudyRecord(
            int index,
            Material material, 
            IEnumerable<StudyFace> fixFaces,
            IEnumerable<StudyFace> loadFaces, 
            StudyMesh mesh) 
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
