using System.Collections.Generic;
using System.Text;

namespace SpaceOptimizerUWP.Models
{
    public class StrainNodeParameters : IParameters
    {
        public int Id { get; set; }
        public float EPSx{ get; set; }
        public float EPSy{ get; set; }
        public float EPSz{ get; set; }
        public float GMxy{ get; set; }
        public float GMyz{ get; set; }
        public float GMxz { get; set; }
        public float ESTRN { get; set; }
        public float SEDENS { get; set; }
        public float ENERGY{ get; set; }
        public float E1{ get; set; }
        public float E2{ get; set; }
        public float E3 { get; set; }

        private Dictionary<string, float> param;
        // показатели по деформации

        public StrainNodeParameters() { }
        public StrainNodeParameters(
            int id,
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
            Id = id;
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
            //string.Join(",\n", Extensions.GetDictString(param));
            return $@"
                ( ""SX"", {EPSx} ),
                ( ""SY"", {EPSy} ),
                ( ""SZ"", {EPSz} ),
                ( ""XY"", {GMxy} ),
                ( ""YZ"", {GMyz} ),
                ( ""XZ"", {GMxz} ),
                ( ""ESTRN"", {ESTRN} ),
                ( ""SEDENS"", {SEDENS} ),
                ( ""ENERGY"", {ENERGY}),
                ( ""E1"", {E1} ),
                ( ""E2"", {E2} ),
                ( ""E3"", {E3} )";
        }
    }

}
