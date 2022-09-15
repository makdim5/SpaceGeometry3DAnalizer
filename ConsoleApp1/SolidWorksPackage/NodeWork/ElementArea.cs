using App2.SolidWorksPackage.NodeWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SolidWorksPackage.NodeWork
{
    public class ElementArea
    {
        public HashSet<Element> elements;

        public ElementArea() { }

        public ElementArea(HashSet<Element> elements)
        {
            this.elements = elements;
        }

        public override string ToString()
        {
            string res = "ElementArea {";

            foreach (Element e in elements)
            {
                res += e.ToString() + "\n";
            }
            res += "}\n\n";

            return res;
        }
    }
}
