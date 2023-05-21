namespace SpaceOptimizerUWP.Models
{
    public class ResearchType
    {
        public const string DBSCAN = "dbscan";
        public const string ADJACMENT = "adjacmentElements";
    }

    public class CutType
    {
        public const string NODE_WAY = "node";
        public const string ELEMENT_WAY = "element";
        public const string POINT_WAY = "point";
    }

    public class FigureCutType
    {
        public const string RECTANGLE = "rect";
        public const string SPHERE = "sphere";
        public const string CUBE = "cube";
    }

    public class NodeCutWay
    {
        public const string FIGURE_WAY = "figure";
        public const string RAVN = "ravn";
    }
}
