using System.Collections.Generic;
using System.Text;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class StrainNodeParameters : IParameters
    {
        public int Id;
        public readonly float EPSx;
        public readonly float EPSy;
        public readonly float EPSz;
        public readonly float GMxy;
        public readonly float GMyz;
        public readonly float GMxz;
        public readonly float ESTRN;
        public readonly float SEDENS;
        public readonly float ENERGY;
        public readonly float E1;
        public readonly float E2;
        public readonly float E3;

        private readonly Dictionary<string, float> param;
        // показатели по деформации
        public StrainNodeParameters(
            float EPSx,
            float EPSy,
            float EPSz,
            float GMxy,
            float GMyz,
            float GMxz,
            float ESTRN,
            float SEDENS,
            float ENERGY,
            float E1,
            float E2,
            float E3)
        {
            this.EPSx = EPSx;
            this.EPSy = EPSy;
            this.EPSz = EPSz;
            this.GMxy = GMxy;
            this.GMyz = GMyz;
            this.GMxz = GMxz;
            this.ESTRN = ESTRN;
            this.SEDENS = SEDENS;
            this.ENERGY = ENERGY;
            this.E1 = E1;
            this.E2 = E2;
            this.E3 = E3;

            this.param = new Dictionary<string, float>
            {
                { "SX", EPSx },
                { "SY", EPSy },
                { "SZ", EPSz },
                { "XY", GMxy },
                { "YZ", GMyz },
                { "XZ", GMxz },
                { "ESTRN", ESTRN },
                { "SEDENS", SEDENS },
                { "ENERGY", ENERGY },
                { "E1", E1 },
                { "E2", E2 },
                { "E3", E3 }
            };

        }
        public Dictionary<string, float> GetParameters()
        {
            return this.param;
        }

        public float GetParam(string param) {
            return this.param[param];
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var param in this.GetParameters())
            {
                sb.AppendLine(param.ToString());
            }
            return sb.ToString();
        }
    }

}
