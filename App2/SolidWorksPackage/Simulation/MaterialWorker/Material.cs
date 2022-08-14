using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolidWorks.Interop.cosworks;

namespace App2.SolidWorksPackage.Simulation.MaterialWorker
{
    public class Material
    {

        // Имя категории
        public readonly string category;

        // Имя материала
        public readonly string name;

        public readonly Dictionary<string, double> physicalProperties;

        //// Модуль упругости
        //public readonly double EX;

        //// Коэффициент Пуассона
        //public readonly double NUXY;

        //// Модуль сдвига
        //public readonly double GXY;

        //// Коэффициент теплового расширения
        //public readonly double ALPX;

        //// Массовая плотность
        //public readonly double DENS;

        //// Теплопроводность
        //public readonly double KX;

        //// Удельная теплоемкость
        //public readonly double C;

        //// Предел прочности при растяжении
        //public readonly double SIGXT;

        //// Предел текучести
        //public readonly double SIGYLD;

        public Material(string category, string name, double[] physicalProperties)
        {

            this.category = category;

            this.name = name;

            this.physicalProperties = new Dictionary<string, double>();

            this.physicalProperties.Add("EX", physicalProperties[0]);
            this.physicalProperties.Add("NUXY", physicalProperties[1]);
            this.physicalProperties.Add("GXY", physicalProperties[2]);
            this.physicalProperties.Add("ALPX", physicalProperties[3]);
            this.physicalProperties.Add("DENS", physicalProperties[4]);
            this.physicalProperties.Add("KX", physicalProperties[5]);
            this.physicalProperties.Add("C", physicalProperties[6]);
            this.physicalProperties.Add("SIGXT", physicalProperties[7]);
            this.physicalProperties.Add("SIGYLD", physicalProperties[8]);

        }

        public Material(
            string category,
            string name,
            double EX,
            double NUXY,
            double GXY,
            double ALPX,
            double DENS,
            double KX,
            double C,
            double SIGXT,
            double SIGYLD)
        {

            this.category = category;

            this.name = name;

            physicalProperties = new Dictionary<string, double>();

            physicalProperties.Add("EX", EX);
            physicalProperties.Add("NUXY", NUXY);
            physicalProperties.Add("GXY", GXY);
            physicalProperties.Add("ALPX", ALPX);
            physicalProperties.Add("DENS", DENS);
            physicalProperties.Add("KX", KX);
            physicalProperties.Add("C", C);
            physicalProperties.Add("SIGXT", SIGXT);
            physicalProperties.Add("SIGYLD", SIGYLD);

        }

        public void SetCWMaterial(CWSolidBody solidBody)
        {

            CWMaterial material = solidBody.GetDefaultMaterial();

            material.MaterialUnits = 0;

            material.MaterialName = name;

            foreach (string physicalPropertieName in physicalProperties.Keys)
            {

                material.SetPropertyByName(physicalPropertieName, physicalProperties[physicalPropertieName], 0);

            }

            solidBody.SetSolidBodyMaterial(material);

        }

        public override string ToString()
        {
            return this.name;
        }

    }
}
