using System;

namespace SolidServer.SolidWorksApplicationPackage
{
    internal class NotSWAppFoundException : Exception
    {
        public override string ToString()
        {
            return "SolidWorks instanse was not found!";
        }
    }

    internal class NotSWDocumentFoundException : Exception
    {
        public override string ToString()
        {
            return "SolidWorks document was not found!";
        }
    }
}
