/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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

        public static List<BuildingElement> SplitBuildingElementsByOverlap(this List<BuildingElement> elementsToSplit)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();
            List<BuildingElement> oriElements = new List<BuildingElement>(elementsToSplit);

            while (elementsToSplit.Count > 0)
            {
                BuildingElement currentElement = elementsToSplit[0];
                List<BuildingElement> overlaps = currentElement.IdentifyOverlaps(elementsToSplit);
                overlaps.AddRange(currentElement.IdentifyOverlaps(rtnElements));

                if (overlaps.Count == 0)
                    rtnElements.Add(currentElement);
                else
                {
                    //Cut the smaller building element out of the bigger one as an opening
                    List<Line> cuttingLines = new List<Line>();
                    foreach(BuildingElement be in overlaps)
                        cuttingLines.AddRange(be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).SubParts());


                    rtnElements.AddRange(currentElement.Split(cuttingLines));
                }

                elementsToSplit.RemoveAt(0);
            }

            return rtnElements;
        }

        public static List<BuildingElement> SplitBuildingElementsByPoints(this List<BuildingElement> elementsToSplit)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();
            List<BuildingElement> oriElements = new List<BuildingElement>(elementsToSplit);

            while(elementsToSplit.Count > 0)
            {
                BuildingElement currentElement = elementsToSplit[0];

                bool wasSplit = false;
                List<BuildingElement> elementSplitResult = new List<BuildingElement>();
                for(int x = 0; x < oriElements.Count; x++)
                {
                    if (oriElements[x].BHoM_Guid == currentElement.BHoM_Guid) continue; //Don't split by the same element

                    //Split this element by each other element in the list
                    elementSplitResult = currentElement.Split(oriElements[x]);

                    if (elementSplitResult.Count > 1)
                    {
                        elementsToSplit.AddRange(elementSplitResult);
                        wasSplit = true;
                        break; //Don't attempt to split this element any further, wait to split its new parts later in the loop...
                    }
                }

                elementsToSplit.RemoveAt(0); //Remove the element we have just worked with, regardless of whether we split it or not
                if (!wasSplit) rtnElements.Add(currentElement); //We have a pure element ready to use
                else
                {
                    //Add the new elements to the list of cutting objects
                    oriElements.RemoveAt(oriElements.IndexOf(oriElements.Where(x => x.BHoM_Guid == currentElement.BHoM_Guid).FirstOrDefault()));
                    oriElements.AddRange(elementSplitResult);
                }
            }

            return rtnElements;
        }

        public static List<BuildingElement> Split(this BuildingElement elementToSplit, List<Line> cuttingLines)
        {
            if (elementToSplit == null || cuttingLines.Count == 0) return new List<BuildingElement> { elementToSplit };

            List<BuildingElement> splitElements = new List<BuildingElement> { elementToSplit };

            foreach (Line l in cuttingLines)
            {
                List<BuildingElement> elementsToSplit = new List<BuildingElement>(splitElements);

                foreach (BuildingElement be in elementsToSplit)
                {
                    Polyline elementToSplitCrv = be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
                    List<Point> cuttingPnts = elementToSplitCrv.LineIntersections(l, true);
                    List<Polyline> cutLines = elementToSplitCrv.SplitAtPoints(cuttingPnts);
                    if (cutLines.Count == 1) continue;

                    splitElements.Remove(be);
                    foreach (Polyline pLine in cutLines)
                    {
                        if (!pLine.IsLinear())
                        {
                            //Only do this for non-straight line cuts
                            List<Point> ctrlPts = pLine.IControlPoints();
                            ctrlPts.Add(ctrlPts[0]); //Close the polyline
                            Polyline completeCrv = BH.Engine.Geometry.Create.Polyline(ctrlPts);

                            BuildingElement cpy = elementToSplit.Copy();
                            cpy.PanelCurve = completeCrv;
                            cpy.CustomData = new Dictionary<string, object>(elementToSplit.CustomData);
                            splitElements.Add(cpy);
                        }
                    }
                }
            }

            return splitElements;
        }

        public static List<BuildingElement> Split(this BuildingElement elementToSplit, BuildingElement cuttingElement)
        {
            if (elementToSplit == null || cuttingElement == null) return new List<BuildingElement> { elementToSplit };

            Polyline elementToSplitCrv = elementToSplit.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
            Polyline cuttingCrv = cuttingElement.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            List<Point> cuttingPts = elementToSplitCrv.LineIntersections(cuttingCrv);

            List<Polyline> cutLines = elementToSplitCrv.SplitAtPoints(cuttingPts);

            if (cutLines.Count == 1) return new List<BuildingElement> { elementToSplit };

            List<BuildingElement> splitElements = new List<BuildingElement>();

            foreach(Polyline pLine in cutLines)
            {
                if(!pLine.IsLinear())
                {
                    //Only do this for non-straight line cuts
                    List<Point> ctrlPts = pLine.IControlPoints();
                    ctrlPts.Add(ctrlPts[0]); //Close the polyline
                    Polyline completeCrv = BH.Engine.Geometry.Create.Polyline(ctrlPts);

                    BuildingElement cpy = elementToSplit.Copy();
                    cpy.PanelCurve = completeCrv;
                    cpy.CustomData = new Dictionary<string, object>(elementToSplit.CustomData);
                    splitElements.Add(cpy);
                }
            }

            return splitElements;
        }

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