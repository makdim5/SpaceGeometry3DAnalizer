using App2.SolidWorksPackage.Simulation.FeatureFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App2.Simulation.Study
{
    public class StudyFace : FeatureFace
    {
        public double force;

        public StudyFace(FeatureFace ff, double force) : base(ff.face, ff.name,ff.color)
        {
            this.force = force;
        }

    }
}
