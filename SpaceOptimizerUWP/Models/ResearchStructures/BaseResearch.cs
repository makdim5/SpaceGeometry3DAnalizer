using System;
using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models
{
    public class BaseResearch
    {
        public int Id { get; set; }
        public MeshParams meshParams { get; set; }
        public string filterParam { get; set; }
        public string materialParam { get; set; }
        public string coef1 { get; set; }
        public string coef2 { get; set; }

        public string squeezeCoef { get; set; }

        public string nodesIntersectionAmount { get; set; }

        public string minSamples { get; set; }
        public string eps { get; set; }

        public BaseResearch() { }

        public BaseResearch(MeshParams meshParams, string filterParam, string materialParam,
            string coef1, string coef2)
        {
            this.meshParams = meshParams;
            this.filterParam = filterParam;
            this.materialParam = materialParam;
            this.coef1 = coef1;
            this.coef2 = coef2;
        }

        public Dictionary<string, object> ToJsonDict()
        {
            return new Dictionary<string, object>(){
                { "meshParams", meshParams.ToJsonDict()},
                { "filterParam", filterParam },
                { "coef1", coef1 },
                { "coef2", coef2 },
                { "materialParam", materialParam },
                { "eps", eps },
                { "minSamples", minSamples},
                {"squeezeCoef", squeezeCoef },
                {"nodesIntersectionAmount", nodesIntersectionAmount }
            };
        }

        public string DetermineType()
        {
            string type = ResearchType.DBSCAN;
            if (squeezeCoef != null && nodesIntersectionAmount != null)
            {
                if (squeezeCoef != "" && nodesIntersectionAmount != "")
                {
                    type = ResearchType.ADJACMENT;
                }
            }
            return type;
        }

        public void CheckIsRightAttributes()
        {
            meshParams.CheckIsRightAttributes();
            double coef1_;
            double coef2_;
            try
            {
                coef1_ = Convert.ToDouble(this.coef1);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert coef1 to double!");
            }

            try
            {
                coef2_ = Convert.ToDouble(this.coef2);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert coef2 to double!");
            }

            if (!(coef1_ > 0 && coef1_ < 1))
            {
                throw new ArgumentException($"coef1 should be in ({0}, {1})," +
                    $" but given {coef1_}!");
            }

            if (!(coef2_ > 0 && coef2_ < 1))
            {
                throw new ArgumentException($"coef2 should be in ({0}, {1})," +
                    $" but given {coef2_}!");
            }

        }

        protected void CheckSpecial()
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
