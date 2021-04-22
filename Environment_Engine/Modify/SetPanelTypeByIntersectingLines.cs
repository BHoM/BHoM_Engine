using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Environment;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        // TODO - fix it, only works for WallInternal PanelType
        public static List<Panel> SetPanelTypeByIntersectingLine(List<Panel> panels, List<Line> intersectingLines, PanelType panelType)
        {
            foreach (Line l in intersectingLines)
            {

                Panel p = panels.Where(x => x.Type == PanelType.WallInternal).Where(x => { List<Point> intersections = x.Polyline().LineIntersections(l); return (intersections != null && intersections.Count > 0); }).FirstOrDefault();
                if (p != null)
                    p.Type = panelType;
            }

            return panels;
        }
    }
}
