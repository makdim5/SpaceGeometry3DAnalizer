using System.Collections.Generic;
using System.Text;

namespace SpaceOptimizerUWP.Models
{
    public class StressNodeParameters : IParameters
    {
        public int Id { get; set; }
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

        // показатели по напряжению
        public StressNodeParameters(
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

            this.param = new Dictionary<string, float>
            {
                { "SX", Sx },
                { "SY", Sy },
                { "SZ", Sz },
                { "XY", Txy },
                { "YZ", Tyz },
                { "XZ", Txz },
                { "P1", P1 },
                { "P2", P2 },
                { "P3", P3 },
                { "VON", VON },
                { "INT", INT }
            };

        }

        public Dictionary<string, float> GetParameters() {
            return this.param;
        }

        public float GetParam(string param)
        {
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
