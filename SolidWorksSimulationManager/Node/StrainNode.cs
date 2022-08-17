using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorksSimulationManager
{
    public class StrainNode : IParameters
    {
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

        public StrainNode(
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

            this.param = new Dictionary<string, float>();

            this.param.Add("SX", EPSx);
            this.param.Add("SY", EPSy);
            this.param.Add("SZ", EPSz);
            this.param.Add("XY", GMxy);
            this.param.Add("YZ", GMyz);
            this.param.Add("XZ", GMxz);
            this.param.Add("ESTRN", ESTRN);
            this.param.Add("SEDENS", SEDENS);
            this.param.Add("ENERGY", ENERGY);
            this.param.Add("E1", E1);
            this.param.Add("E2", E2);
            this.param.Add("E3", E3);

        }
        public Dictionary<string, float> GetParameters()
        {
            return this.param;
        }

        public float GetParam(string param) {
            return this.param[param];
        }
    }

}
