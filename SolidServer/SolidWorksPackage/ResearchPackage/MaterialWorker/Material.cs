using System.Collections.Generic;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class Material
    {

        public readonly string category;
        public readonly string name;
        public readonly Dictionary<string, double> physicalProperties;

        public Material() { }

        public Material(string category, string name, double[] physicalProperties)
        {
            this.category = category;
            this.name = name;
            this.physicalProperties = new Dictionary<string, double>
            {
                { "EX", physicalProperties[0] }, // Модуль упругости
                { "NUXY", physicalProperties[1] }, // Коэффициент Пуассона
                { "GXY", physicalProperties[2] }, // Модуль сдвига
                { "ALPX", physicalProperties[3] }, // Коэффициент теплового расширения
                { "DENS", physicalProperties[4] }, // Массовая плотность
                { "KX", physicalProperties[5] }, // Теплопроводность
                { "C", physicalProperties[6] }, // Удельная теплоемкость
                { "SIGXT", physicalProperties[7] }, // Предел прочности при растяжении
                { "SIGYLD", physicalProperties[8] } // Предел текучести
            };
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

            physicalProperties = new Dictionary<string, double>
            {
                { "EX", EX },
                { "NUXY", NUXY },
                { "GXY", GXY },
                { "ALPX", ALPX },
                { "DENS", DENS },
                { "KX", KX },
                { "C", C },
                { "SIGXT", SIGXT },
                { "SIGYLD", SIGYLD }
            };
        }

        public override string ToString()
        {
            return name;
        }
    }
}
