using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models
{
    interface IParameters
    {
        public Dictionary<string, float> GetParameters();

        public float GetParam(string param);

    }
}
