using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /******************************************/
        /****      Element control points      ****/
        /******************************************/

        public static List<Point> ControlPoints(this PanelPlanar panel, bool externalOnly = false)
        {
            List<Point> pts = panel.ExternalEdges.ControlPoints();
            if (!externalOnly)
            {
                foreach (Opening o in panel.Openings)
                {
                    pts.AddRange(o.Edges.ControlPoints());
                }
            }
            return pts;
        }

        /******************************************/

        public static List<Point> ControlPoints(this Opening opening)
        {
            return opening.Edges.ControlPoints();
        }

        /******************************************/

        public static List<Point> ControlPoints(this List<Edge> edges)
        {
            List<Point> pts = edges.Select(e => (e.Curve as Line).Start.Clone()).ToList();
            return pts;
        }

        /******************************************/

        public static List<Point> ControlPoints(this Bar bar)
        {
            return new List<Point> { bar.StartNode.Position.Clone(), bar.EndNode.Position.Clone() };
        }

        /******************************************/
    }
}
