using System;
namespace SpaceOptimizerUWP.Models
{
    public class AdjacmentResearch : BaseResearch
    {
        public string squeezeCoef { get; set; }

        public string nodesIntersectionAmount { get; set; }
        public AdjacmentResearch() { }

        public AdjacmentResearch(MeshParams meshParams, string filterParam, string materialParam,
            string coef1, string coef2, string squeezeCoef, string nodesIntersectionAmount
            ) : base(meshParams, filterParam, materialParam, coef1, coef2)
        {
            this.squeezeCoef = squeezeCoef;
            this.nodesIntersectionAmount = nodesIntersectionAmount;
        }

        protected override void CheckSpecial()
        {
            double squeezeCoef_;
            int nodesIntersectionAmount_;

            try
            {
                squeezeCoef_ = Convert.ToDouble(this.squeezeCoef);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert squeezeCoef to double!");
            }

            try
            {
                nodesIntersectionAmount_ = Convert.ToInt32(this.nodesIntersectionAmount);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert nodesIntersectionAmount to int!");
            }
        }
    }
}
