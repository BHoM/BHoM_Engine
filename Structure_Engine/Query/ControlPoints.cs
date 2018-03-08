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
            List<Point> pts = panel.ExternalEdges.Select(e => (e.Curve as Line).Start.Clone()).ToList();
            if (!externalOnly)
            {
                foreach (Opening o in panel.Openings)
                {
                    pts.AddRange(o.Edges.Select(e => (e.Curve as Line).Start.Clone()).ToList());
                }
            }
            return pts;
        }

        /******************************************/

        public static List<Point> ControlPoints(this Opening opening)
        {
            List<Point> pts = opening.Edges.Select(e => (e.Curve as Line).Start.Clone()).ToList();
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
