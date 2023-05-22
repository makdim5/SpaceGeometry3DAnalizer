using System.Collections.Generic;

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
        public static List<string> NAMES = new List<string>() { NODE_WAY, ELEMENT_WAY, POINT_WAY };
    }

    public class FigureCutType
    {
        public const string RECTANGLE = "rect";
        public const string SPHERE = "sphere";
        public const string CUBE = "cube";
        public static List<string> NAMES = new List<string>(){ RECTANGLE , SPHERE, CUBE};
    }

    public class NodeCutWay
    {
        public const string FIGURE_WAY = "figure";
        public const string RAVN = "ravn";
        public static List<string> NAMES = new List<string>() { FIGURE_WAY, RAVN };
    }
}
