using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        public static List<Panel> FlipPanels(List<Panel> panels)
        {
            List<Panel> modifiedPanels = new List<Panel>();
            foreach (Panel p in panels)
            {
                
                Panel p2 = p.DeepClone();
                if (!p2.NormalAwayFromSpace(panels))
                    p2.ExternalEdges = p2.Polyline().Flip().ToEdges();

                List<Opening> openings = p2.Openings.Select(x => x.DeepClone()).ToList();
                for (int x = 0; x < openings.Count; x++)
                {
                    if (!openings[x].Polyline().NormalAwayFromSpace(panels))
                        openings[x].Edges = openings[x].Polyline().Flip().ToEdges();
                }

                p2.Openings = openings;
                modifiedPanels.Add(p2);
            }

            return modifiedPanels;
        }
    }
}
