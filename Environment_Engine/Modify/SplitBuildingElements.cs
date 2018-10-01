using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<BuildingElement> SplitBuildingElements(this List<BuildingElement> elementsToSplit)
        {
            //Go through all building elements and compare to see if any should be split into smaller building elements
            List<BuildingElement> rtn = new List<BuildingElement>();

            Dictionary<BuildingElement, List<BuildingElement>> overlaps = new Dictionary<BuildingElement, List<BuildingElement>>();

            foreach (BuildingElement be in elementsToSplit)
                overlaps.Add(be, be.IdentifyOverlaps(elementsToSplit));

            foreach (KeyValuePair<BuildingElement, List<BuildingElement>> kvp in overlaps)
            {
                Dictionary<BuildingElement, List<BuildingElement>> rtn2 = new Dictionary<BuildingElement, List<BuildingElement>>();
                Dictionary<BuildingElement, List<Polyline>> replacementGeom = new Dictionary<BuildingElement, List<Polyline>>();
                Polyline be1P = kvp.Key.PanelCurve.ICollapseToPolyline(Tolerance.Angle);

                foreach (BuildingElement be2 in kvp.Value)
                {
                    Polyline be2p = be2.PanelCurve.ICollapseToPolyline(Tolerance.Angle);

                    Dictionary<BuildingElement, List<Polyline>> geomBuild = new Dictionary<BuildingElement, List<Polyline>>();
                    geomBuild.Add(kvp.Key, new List<Polyline>());
                    geomBuild.Add(be2, new List<Polyline>());

                    List<Polyline> intersections = be1P.BooleanIntersection(be2p);
                    foreach(Polyline p in intersections)
                    {
                        geomBuild[kvp.Key].AddRange(be1P.SplitAtPoints(p.ControlPoints));
                        geomBuild[be2].AddRange(be2p.SplitAtPoints(p.ControlPoints));
                    }

                    foreach(KeyValuePair<BuildingElement, List<Polyline>> kvp2 in geomBuild)
                    {
                        List<Polyline> remove = new List<Polyline>();
                        foreach(Polyline p5 in kvp2.Value)
                        {
                            bool isNotIn = true;
                            foreach (Point px in p5.ControlPoints)
                            {
                                foreach (Polyline p6 in intersections)
                                {
                                    if (p6.ControlPoints.Contains(px))
                                        isNotIn = false;

                                    if (!isNotIn) break;
                                }
                                if (!isNotIn) break;
                            }

                            if (!isNotIn)
                                remove.Add(p5);
                        }

                        foreach (Polyline l5 in remove)
                            kvp2.Value.Remove(l5);
                    }

                    foreach (KeyValuePair<BuildingElement, List<Polyline>> kvp3 in geomBuild)
                    {
                        foreach (Polyline p5 in kvp3.Value)
                        {
                            while (p5.ControlPoints.Last().Distance(p5.ControlPoints.First()) > 0.01)
                            {
                                bool addedPoint = false;
                                foreach (Point px in intersections[0].ControlPoints)
                                {
                                    if (!p5.ControlPoints.Contains(px) && !kvp3.Key.PanelCurve.ICollapseToPolyline(1e-06).ControlPoints.Contains(px) && px.Match2Of3(p5.ControlPoints.Last()))
                                    {
                                        p5.ControlPoints.Add(px);
                                        addedPoint = true;
                                    }
                                }
                                if (!addedPoint)
                                    p5.ControlPoints.Add(p5.ControlPoints[0]);
                            }
                        }
                    }

                    foreach (KeyValuePair<BuildingElement, List<Polyline>> kvp4 in geomBuild)
                    {
                        if (!replacementGeom.ContainsKey(kvp4.Key))
                            replacementGeom.Add(kvp4.Key, new List<Polyline>());
                        replacementGeom[kvp4.Key].AddRange(kvp4.Value);
                    }
                }

                //Make the new BE from the new polylines
                if (replacementGeom.Count == 0) rtn2.Add(kvp.Key, new List<BuildingElement>());
                foreach (KeyValuePair<BuildingElement, List<Polyline>> kvp6 in replacementGeom)
                {
                    BuildingElement ori = kvp6.Key;
                    if (kvp6.Value.Count > 1)
                    {
                        //Only do this if we have more than 1 BE to replace
                        foreach (Polyline p in kvp6.Value)
                        {
                            BuildingElement newBE = ori.GetShallowClone() as BuildingElement;
                            if (newBE.PanelCurve == null)
                                newBE.PanelCurve = ori.PanelCurve;
                            if (newBE.BuildingElementProperties == null)
                                newBE.BuildingElementProperties = ori.BuildingElementProperties.GetShallowClone() as BH.oM.Environment.Properties.BuildingElementProperties;

                            newBE.PanelCurve = p;

                            if (!rtn2.ContainsKey(ori))
                                rtn2.Add(ori, new List<BuildingElement>());
                            rtn2[ori].Add(newBE);
                        }
                    }
                    else if (kvp6.Value.Count == 0)
                    {
                        //This BE was cut in such a way that it ended up with no polygon - slightly problematic but basically the entire polygon was the intersection with the other BE - so add the old BE back
                        if (!rtn2.ContainsKey(ori))
                            rtn2.Add(ori, new List<BuildingElement>());
                        rtn2[ori].Add(ori);
                    }
                    else if (kvp6.Value.Count == 1)
                        rtn2.Add(ori, new List<BuildingElement>());
                }

                //Update the building elements
                if (rtn2[kvp.Key].Count == 0)
                    rtn.Add(kvp.Key); //Not split at all
                else
                    rtn.AddRange(rtn2[kvp.Key]);
            }
                       

            /*for(int x = 0; x < elementsToSplit.Count; x++)
            {
                List<BuildingElement> overlappingElements = elementsToSplit[x].IdentifyOverlaps(elementsToSplit);
                if (overlappingElements.Count == 0) rtn.Add(elementsToSplit[x]);
                else
                {
                    //This element overlaps with some elements in the list - split them up into new elements
                    foreach(BuildingElement be in overlappingElements)
                    {
                        //Split the original element by this BE
                        Polyline original = elementsToSplit[x].PanelCurve.ICollapseToPolyline(Tolerance.Angle);
                        Polyline cutting = be.PanelCurve.ICollapseToPolyline(Tolerance.Angle);

                        List<Polyline> intersections = original.BooleanIntersection(cutting);
                        foreach(Polyline p in intersections)
                        {
                            List<Polyline> splits = original.SplitAtPoints(p.ControlPoints);
                            List<Point> points = new List<Point>();
                            foreach (Polyline p2 in splits)
                                points.AddRange(p2.ControlPoints);

                            original = BH.Engine.Geometry.Create.Polyline(points);
                        }

                        elementsToSplit[x].PanelCurve = original; //Reset the cut geometry to the BE ready for the next overlap
                    }
                }

                rtn.Add(elementsToSplit[x]);
            }*/

            return rtn;
        }

        public static bool Match2Of3(this Point pt, Point comp)
        {
            bool match2 = false;
            if (pt.X == comp.X)
            {
                if (pt.Y == comp.Y)
                    match2 = true;
                else if (pt.Z == comp.Z)
                    match2 = true;
            }
            else if (pt.Y == comp.Y && pt.Z == comp.Z)
                match2 = true;

            return match2;
        }
    }
}