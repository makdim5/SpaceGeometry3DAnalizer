using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidServer.exceptions
{
    internal class NotSWAppFoundException : Exception
    {
        public override string ToString()
        {
            return "SolidWorks instanse was not found!";
        }
    }
}
