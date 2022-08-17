using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorksSimulationManager
{
    public class StressNode : IParameters
    {

        public readonly float Sx;
        public readonly float Sy;
        public readonly float Sz;
        public readonly float Txy;
        public readonly float Tyz;
        public readonly float Txz;
        public readonly float P1;
        public readonly float P2;
        public readonly float P3;
        public readonly float VON;
        public readonly float INT;

        private readonly Dictionary<string,float> param;

        public StressNode(
            float Sx,
            float Sy,
            float Sz,
            float Txy,
            float Tyz,
            float Txz,
            float P1,
            float P2,
            float P3,
            float VON,
            float INT) {

            this.Sx = Sx;
            this.Sy = Sy;
            this.Sz = Sz;
            this.Txy = Txy;
            this.Tyz = Tyz;
            this.Txz = Txz;
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
            this.VON = VON;
            this.INT = INT;

            this.param = new Dictionary<string, float>();

            this.param.Add("SX", Sx);
            this.param.Add("SY", Sy);
            this.param.Add("SZ", Sz);
            this.param.Add("XY", Txy);
            this.param.Add("YZ", Tyz);
            this.param.Add("XZ", Txz);
            this.param.Add("P1", P1);
            this.param.Add("P2", P2);
            this.param.Add("P3", P3);
            this.param.Add("VON", VON);
            this.param.Add("INT", INT);

        }

        public Dictionary<string, float> GetParameters() {
            return this.param;
        }

        public float GetParam(string param)
        {
            return this.param[param];
        }

    }
}
