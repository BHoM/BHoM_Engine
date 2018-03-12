using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /******************************************/
        /****          Panel outline           ****/
        /******************************************/

        public static Polyline Outline(this PanelPlanar panel)
        {
            List<Point> pPts = panel.ControlPoints(true);
            pPts.Add(pPts[0]);
            return new Polyline { ControlPoints = pPts };
        }


        /******************************************/
        /****         Opening outline          ****/
        /******************************************/

        public static Polyline Outline(this Opening opening)
        {
            List<Point> pPts = opening.ControlPoints();
            pPts.Add(pPts[0]);
            return new Polyline { ControlPoints = pPts };
        }

        /******************************************/

        public static Polyline Outline(this List<Edge> edges)
        {
            List<Point> pPts = edges.ControlPoints();
            pPts.Add(pPts[0]);
            return new Polyline { ControlPoints = pPts };
        }

        /******************************************/
    }
}
