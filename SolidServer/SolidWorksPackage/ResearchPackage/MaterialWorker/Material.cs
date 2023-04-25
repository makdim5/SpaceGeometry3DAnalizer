using System.Collections.Generic;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class Material
    {

        public readonly string category;
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

        public Material() { }

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

        public override string ToString()
        {
            return this.name;
        }

    }
}
