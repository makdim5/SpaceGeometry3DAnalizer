using System.Collections.Generic;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    interface IParameters
    {
        public Dictionary<string, float> GetParameters();

        public float GetParam(string param);

    }
}
