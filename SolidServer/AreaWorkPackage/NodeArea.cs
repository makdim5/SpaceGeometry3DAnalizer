using SolidServer.SolidWorksPackage.ResearchPackage;
using System.Collections.Generic;


namespace SolidServer.AreaWorkPackage
{
    public class NodeArea
    {
        public HashSet<Node> nodes;
        public NodeArea() { }

        public NodeArea(HashSet<Node> nodes)
        {
            this.nodes = nodes;
        }
    }
}
