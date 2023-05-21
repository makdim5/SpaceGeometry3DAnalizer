using System;

namespace SpaceOptimizerUWP.Models
{
    public abstract class BaseResearch
    {
        public MeshParams meshParams {get; set;}
        public string filterParam { get; set; }
        public string materialParam { get; set; }
        public string coef1 { get; set; }
        public string coef2 { get; set; }

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

            CheckSpecial();
        }

        protected abstract void CheckSpecial();
    }
}
