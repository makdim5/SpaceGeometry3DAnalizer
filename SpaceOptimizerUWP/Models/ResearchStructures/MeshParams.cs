using System;
using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models
{
    public class MeshParams
    {
        public int Id { get; set; }
        public string Quality { get; set; }
        public string UseJacobianCheck { get; set; }
        public string MesherType { get; set; }
        public string MinElementsInCircle { get; set; }
        public string GrowthRatio { get; set; }
        public string SaveSettingsWithoutMeshing { get; set; }
        public string Unit { get; set; }

        public MeshParams() { }

        public MeshParams(string Quality, string UseJacobianCheck, string MesherType,string MinElementsInCircle,
            string GrowthRatio, string SaveSettingsWithoutMeshing, string Unit)
        {
            this.Quality = Quality;
            this.UseJacobianCheck = UseJacobianCheck;
            this.MesherType = MesherType;
            this.MinElementsInCircle = MinElementsInCircle;
            this.GrowthRatio = GrowthRatio;
            this.SaveSettingsWithoutMeshing = SaveSettingsWithoutMeshing;
            this.Unit = Unit;
        }

        public Dictionary<string, string> ToJsonDict()
        {
            return new Dictionary<string, string>()
                    {
                        {"Quality", Quality },
                        {"UseJacobianCheck", UseJacobianCheck },
                        {"MesherType", MesherType },
                        {"MinElementsInCircle", MinElementsInCircle },
                        {"GrowthRatio", GrowthRatio },
                        {"SaveSettingsWithoutMeshing", SaveSettingsWithoutMeshing },
                        {"Unit",Unit }
                    };
        }

        public void CheckIsRightAttributes()
        {
            int quality;
            int useJacobianCheck;
            int mesherType;
            int minElementsInCircle;
            double growthRatio;
            int saveSettingsWithoutMeshing;
            int unit;

            try
            {
                quality = Convert.ToInt32(this.Quality);
            } catch
            {
                throw new ArgumentException("Impossible to convert quality to int!");
            }

            try
            {
                useJacobianCheck = Convert.ToInt32(this.UseJacobianCheck);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert useJacobianCheck to int!");
            }

            try
            {
                mesherType = Convert.ToInt32(this.MesherType);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert mesherType to int!");
            }

            try
            {
                minElementsInCircle = Convert.ToInt32(this.MinElementsInCircle);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert minElementsInCircle to int!");
            }

            try
            {
                growthRatio = Convert.ToDouble(this.GrowthRatio);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert growthRatio to double!");
            }

            try
            {
                saveSettingsWithoutMeshing = Convert.ToInt32(this.SaveSettingsWithoutMeshing);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert saveSettingsWithoutMeshing to int!");
            }

            try
            {
                unit = Convert.ToInt32(this.Unit);
            }
            catch
            {
                throw new ArgumentException("Impossible to convert unit to int!");
            }

            if (!(saveSettingsWithoutMeshing >= 0 && saveSettingsWithoutMeshing < 2))
            {
                throw new ArgumentException($"saveSettingsWithoutMeshing should be in [{0}, {2})," +
                    $" but given {saveSettingsWithoutMeshing}!");
            }
        }
    }
}
