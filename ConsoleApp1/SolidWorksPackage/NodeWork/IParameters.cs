using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage.NodeWork
{
    interface IParameters
    {
        public Dictionary<string, float> GetParameters();

        public float GetParam(string param);

    }
}
