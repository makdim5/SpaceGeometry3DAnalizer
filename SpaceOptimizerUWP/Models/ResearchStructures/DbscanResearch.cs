using System;

namespace SpaceOptimizerUWP.Models
{
    public class DbscanResearch : BaseResearch
    {
        public string minSamples;
        public string eps;

        public DbscanResearch() { }

        public DbscanResearch(MeshParams meshParams, string filterParam, string materialParam,
            string coef1, string coef2, string minSamples, string eps
            ) : base(meshParams, filterParam, materialParam, coef1, coef2)
        {
            this.minSamples = minSamples;
            this.eps = eps;
        }

        protected override void CheckSpecial()
        {
            int minSamples_;
            double eps_;

            try
            {
                eps_ = Convert.ToDouble(this.eps);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert eps to double!");
            }

            try
            {
                minSamples_ = Convert.ToInt32(this.coef1);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert minSamples to int!");
            }
        }
    }
}
