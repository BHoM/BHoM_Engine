using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;

using BH.oM.Geometry;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<BuildingElement>> MergeBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            foreach(List<BuildingElement> space in elements)
            {

                foreach(BuildingElement be in space)
                {

                }
            }

            return spaces;
        }

        public static List<BuildingElement> MergeBuildingElements(this List<BuildingElement> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            while (elements.Count > 0)
            {
                BuildingElement current = elements[0];
                List<BuildingElement> connected = current.ConnectedElementsByEdge(elements).Where(x => x.MatchAdjacencies(current)).Where(x => x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsCoplanar(current.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle))).ToList();

                if (connected.Count == 0)
                    rtn.Add(current);
                else
                {
                    foreach(BuildingElement be in connected)
                    {
                        if (current.CanMerge(be))
                        {
                            current.PanelCurve = (current.MergedShell(be) == null ? current.PanelCurve : current.MergedShell(be));
                            elements.Remove(be);
                        }
                    }
                    rtn.Add(current);
                }

                elements.RemoveAt(0);
            }

            return rtn;
        }

        public static bool CanMerge(this BuildingElement element, BuildingElement attemptElement)
        {
            //Put the polycurves together - do they form a planar closed shell?
            Polyline pLine1 = element.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
            Polyline pLine2 = attemptElement.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            List<Line> subParts = pLine1.SubParts();
            subParts.AddRange(pLine2.SubParts());

            List<Line> c = subParts.Distinct().ToList();

            List<Polyline> join = c.Join();
            if (join.Count > 1) return false;

            return join[0].IsClosed();
        }

        public static Polyline MergedShell(this BuildingElement element, BuildingElement attemptElement)
        {
            if (!element.CanMerge(attemptElement)) return null;

            Polyline pLine1 = element.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
            Polyline pLine2 = attemptElement.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            List<Line> subParts = pLine1.SubParts();
            subParts.AddRange(pLine2.SubParts());

            List<Line> c = subParts.Distinct().ToList();

            List<Polyline> join = c.Join();
            if (join.Count != 1) return null;
            return join[0];
        }

        /***************************************************/
    }
}
