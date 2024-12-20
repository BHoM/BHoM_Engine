/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels that are split by a collection of provided cutting lines")]
        [Input("panel", "An Environment Panel to split")]
        [Input("cuttingLines", "A collection of BHoM Geometry Lines to split the panels by")]
        [Output("panels", "A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this Panel panel, List<Line> cuttingLines)
        {
            Panel clone = panel.DeepClone<Panel>();
            if (clone == null || cuttingLines.Count == 0) return new List<Panel> { clone };

            List<Panel> splitElements = new List<Panel> { clone };

            foreach (Line l in cuttingLines)
            {
                List<Panel> elementsToSplit = new List<Panel>(splitElements);

                foreach (Panel be in elementsToSplit)
                {
                    Polyline elementToSplitCrv = be.Polyline();
                    List<Point> cuttingPnts = elementToSplitCrv.LineIntersections(l, true);
                    List<Polyline> cutLines = elementToSplitCrv.SplitAtPoints(cuttingPnts);
                    
                    if (cutLines.Count == 1)
                        continue;

                    splitElements.Remove(be);
                    foreach (Polyline pLine in cutLines)
                    {
                        if (!pLine.IsLinear())
                        {
                            //Only do this for non-straight line cuts
                            List<Point> ctrlPts = pLine.IControlPoints();
                            ctrlPts.Add(ctrlPts[0]); //Close the polyline
                            Polyline completeCrv = BH.Engine.Geometry.Create.Polyline(ctrlPts);

                            Panel cpy = clone.Copy();
                            cpy.ExternalEdges = completeCrv.ToEdges();
                            splitElements.Add(cpy);
                        }
                    }
                }
            }

            return splitElements;
        }

        [Description("Returns a collection of Environment Panels that are split")]
        [Input("panel", "An Environment Panel to split")]
        [Input("cuttingPanel", "An Environment Panel to split the first panel by")]
        [Output("panels", "A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this Panel panel, Panel cuttingPanel)
        {
            Panel clone = panel.DeepClone<Panel>();
            if (clone == null || cuttingPanel == null) return new List<Panel> { clone };

            Polyline elementToSplitCrv = clone.Polyline();
            Polyline cuttingCrv = cuttingPanel.Polyline();

            List<Point> cuttingPts = elementToSplitCrv.LineIntersections(cuttingCrv);

            List<Polyline> cutLines = elementToSplitCrv.SplitAtPoints(cuttingPts);

            if (cutLines.Count == 1) return new List<Panel> { clone };

            List<Panel> splitElements = new List<Panel>();

            foreach (Polyline pLine in cutLines)
            {
                if (!pLine.IsLinear())
                {
                    //Only do this for non-straight line cuts
                    List<Point> ctrlPts = pLine.IControlPoints();
                    ctrlPts.Add(ctrlPts[0]); //Close the polyline
                    Polyline completeCrv = BH.Engine.Geometry.Create.Polyline(ctrlPts);

                    Panel cpy = clone.Copy();
                    cpy.ExternalEdges = completeCrv.ToEdges();
                    cpy.CustomData = new Dictionary<string, object>(clone.CustomData);
                    splitElements.Add(cpy);
                }
            }

            return splitElements;
        }

        [Description("Returns a collection of Environment Panels that are split by their overlapping elements")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("panels", "A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this List<Panel> panels)
        {
            //Go through all building elements and compare to see if any should be split into smaller building elements
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            List<Panel> rtn = new List<Panel>();

            Dictionary<Panel, List<Panel>> overlaps = new Dictionary<Panel, List<Panel>>();

            foreach (Panel be in clones)
                overlaps.Add(be, be.IdentifyOverlaps(clones));

            foreach (KeyValuePair<Panel, List<Panel>> kvp in overlaps)
            {
                Dictionary<Panel, List<Panel>> rtn2 = new Dictionary<Panel, List<Panel>>();
                Dictionary<Panel, List<Polyline>> replacementGeom = new Dictionary<Panel, List<Polyline>>();
                Polyline be1P = kvp.Key.Polyline();

                foreach (Panel be2 in kvp.Value)
                {
                    Polyline be2p = be2.Polyline();

                    Dictionary<Panel, List<Polyline>> geomBuild = new Dictionary<Panel, List<Polyline>>();
                    geomBuild.Add(kvp.Key, new List<Polyline>());
                    geomBuild.Add(be2, new List<Polyline>());

                    List<Polyline> intersections = be1P.BooleanIntersection(be2p);
                    foreach (Polyline p in intersections)
                    {
                        geomBuild[kvp.Key].AddRange(be1P.SplitAtPoints(p.ControlPoints));
                        geomBuild[be2].AddRange(be2p.SplitAtPoints(p.ControlPoints));
                    }

                    foreach (KeyValuePair<Panel, List<Polyline>> kvp2 in geomBuild)
                    {
                        List<Polyline> remove = new List<Polyline>();
                        foreach (Polyline p5 in kvp2.Value)
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

                    foreach (KeyValuePair<Panel, List<Polyline>> kvp3 in geomBuild)
                    {
                        foreach (Polyline p5 in kvp3.Value)
                        {
                            while (p5.ControlPoints.Last().Distance(p5.ControlPoints.First()) > 0.01)
                            {
                                bool addedPoint = false;
                                foreach (Point px in intersections[0].ControlPoints)
                                {
                                    if (!p5.ControlPoints.Contains(px) && !kvp3.Key.Polyline().ControlPoints.Contains(px) && px.MatchPointOn2Of3(p5.ControlPoints.Last()))
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

                    foreach (KeyValuePair<Panel, List<Polyline>> kvp4 in geomBuild)
                    {
                        if (!replacementGeom.ContainsKey(kvp4.Key))
                            replacementGeom.Add(kvp4.Key, new List<Polyline>());
                        replacementGeom[kvp4.Key].AddRange(kvp4.Value);
                    }
                }

                //Make the new BE from the new polylines
                if (replacementGeom.Count == 0) rtn2.Add(kvp.Key, new List<Panel>());
                foreach (KeyValuePair<Panel, List<Polyline>> kvp6 in replacementGeom)
                {
                    Panel ori = kvp6.Key;
                    if (kvp6.Value.Count > 1)
                    {
                        //Only do this if we have more than 1 BE to replace
                        foreach (Polyline p in kvp6.Value)
                        {
                            Panel newBE = ori.ShallowClone();
                            if (newBE.ExternalEdges.Count == 0)
                                newBE.ExternalEdges = new List<Edge>(ori.ExternalEdges);

                            newBE.ExternalEdges = p.ToEdges();

                            if (!rtn2.ContainsKey(ori))
                                rtn2.Add(ori, new List<Panel>());
                            rtn2[ori].Add(newBE);
                        }
                    }
                    else if (kvp6.Value.Count == 0)
                    {
                        //This BE was cut in such a way that it ended up with no polygon - slightly problematic but basically the entire polygon was the intersection with the other BE - so add the old BE back
                        if (!rtn2.ContainsKey(ori))
                            rtn2.Add(ori, new List<Panel>());
                        rtn2[ori].Add(ori);
                    }
                    else if (kvp6.Value.Count == 1)
                        rtn2.Add(ori, new List<Panel>());
                }

                //Update the building elements
                if (rtn2[kvp.Key].Count == 0)
                    rtn.Add(kvp.Key); //Not split at all
                else
                    rtn.AddRange(rtn2[kvp.Key]);
            }

            return rtn;
        }
    }
}





