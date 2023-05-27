using System.Collections.Generic;
using System.Text;

namespace SpaceOptimizerUWP.Models
{
    public class StressNodeParameters : IParameters
    {
        public int Id { get; set; }
        public float Sx{ get; set; }
        public float Sy{ get; set; }
        public float Sz { get; set; }
        public float Txy{ get; set; }
        public float Tyz{ get; set; }
        public float Txz { get; set; }
        public float P1{ get; set; }
        public float P2{ get; set; }
        public float P3 { get; set; }
        public float VON{ get; set; }
        public float INT { get; set; }

        private Dictionary<string, float> param;

        public StressNodeParameters() { }
        // показатели по напряжению
        public StressNodeParameters(
            int id,
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
            Id = id;
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
            //string.Join(",\n", Extensions.GetDictString(param));
            return $@"
                ( ""SX"", {Sx} ),
                ( ""SY"", {Sy} ),
                ( ""SZ"", {Sz} ),
                ( ""XY"", {Txy} ),
                ( ""YZ"", {Tyz} ),
                ( ""XZ"", {Txz}),
                ( ""P1"", {P1} ),
                ( ""P2"", {P2} ),
                ( ""P3"", {P3} ),
                ( ""VON"", {VON} ),
                ( ""INT"", {INT} )";
        }
    }
}
