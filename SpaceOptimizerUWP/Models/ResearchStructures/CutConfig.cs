﻿using System.Collections.Generic;

namespace SpaceOptimizerUWP.Models
{
    public class CutConfig
    {
        public string cutType { get; set; }
        public string nodeCutWay { get; set; }

        public string figureType { get; set; }
        public CutConfig() { }

        public CutConfig(string cutType, string nodeCutWay, string figureType)
        {
            this.cutType = cutType;
            this.nodeCutWay = nodeCutWay;
            this.figureType = figureType;
        }

        public Dictionary<string, string> ToJsonDict()
        {
            return new Dictionary<string, string>() {
                {"cutType", cutType},
                {"nodeCutWay", nodeCutWay},
                {"figureType", figureType }
            };
        }
    }
}
