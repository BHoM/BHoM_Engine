/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Modify.SplitPanelsByOverlap => Returns a collection of Environment Panels that have been split if any of them overlap each other to ensure no panels overlap")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("A collection of Environment Panels that do not overlap")]
        public static List<Panel> SplitPanelsByOverlap(this List<Panel> panels)
        {
            List<Panel> rtnElements = new List<Panel>();
            List<Panel> oriElements = new List<Panel>(panels);

            while (panels.Count > 0)
            {
                Panel currentElement = panels[0];
                List<Panel> overlaps = currentElement.IdentifyOverlaps(panels);
                overlaps.AddRange(currentElement.IdentifyOverlaps(rtnElements));

                if (overlaps.Count == 0)
                    rtnElements.Add(currentElement);
                else
                {
                    //Cut the smaller building element out of the bigger one as an opening
                    List<Line> cuttingLines = new List<Line>();
                    foreach(Panel be in overlaps)
                        cuttingLines.AddRange(be.ToPolyline().SubParts());


                    rtnElements.AddRange(currentElement.Split(cuttingLines));
                }

                panels.RemoveAt(0);
            }

            return rtnElements;
        }

        [Description("BH.Engine.Environment.Modify.SplitPanelsByPoints => Returns a collection of Environment Panels that are split by their edge points ensuring panels which span multiple spaces are split and have adjacencies assigned correctly")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("A collection of Environment Panels that have been split")]
        public static List<Panel> SplitPanelsByPoints(this List<Panel> panels)
        {
            List<Panel> rtnElements = new List<Panel>();
            List<Panel> oriElements = new List<Panel>(panels);

            while(panels.Count > 0)
            {
                Panel currentElement = panels[0];

                bool wasSplit = false;
                List<Panel> elementSplitResult = new List<Panel>();
                for(int x = 0; x < oriElements.Count; x++)
                {
                    if (oriElements[x].BHoM_Guid == currentElement.BHoM_Guid) continue; //Don't split by the same element

                    //Split this element by each other element in the list
                    elementSplitResult = currentElement.Split(oriElements[x]);

                    if (elementSplitResult.Count > 1)
                    {
                        panels.AddRange(elementSplitResult);
                        wasSplit = true;
                        break; //Don't attempt to split this element any further, wait to split its new parts later in the loop...
                    }
                }

                panels.RemoveAt(0); //Remove the element we have just worked with, regardless of whether we split it or not
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

        [Description("BH.Engine.Environment.Modify.Split => Returns a collection of Environment Panels that are split by a collection of provided cutting lines")]
        [Input("panel", "An Environment Panel to split")]
        [Input("cuttingLines", "A collection of BHoM Geometry Lines to split the panels by")]
        [Output("A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this Panel panel, List<Line> cuttingLines)
        {
            if (panel == null || cuttingLines.Count == 0) return new List<Panel> { panel };

            List<Panel> splitElements = new List<Panel> { panel };

            foreach (Line l in cuttingLines)
            {
                List<Panel> elementsToSplit = new List<Panel>(splitElements);

                foreach (Panel be in elementsToSplit)
                {
                    Polyline elementToSplitCrv = be.ToPolyline();
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

                            Panel cpy = panel.Copy();
                            cpy.ExternalEdges = completeCrv.ToEdges();
                            cpy.CustomData = new Dictionary<string, object>(panel.CustomData);
                            splitElements.Add(cpy);
                        }
                    }
                }
            }

            return splitElements;
        }

        [Description("BH.Engine.Environment.Modify.Split => Returns a collection of Environment Panels that are split")]
        [Input("panel", "An Environment Panel to split")]
        [Input("cuttingPanel", "An Environment Panel to split the first panel by")]
        [Output("A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this Panel panel, Panel cuttingPanel)
        {
            if (panel == null || cuttingPanel == null) return new List<Panel> { panel };

            Polyline elementToSplitCrv = panel.ToPolyline();
            Polyline cuttingCrv = cuttingPanel.ToPolyline();

            List<Point> cuttingPts = elementToSplitCrv.LineIntersections(cuttingCrv);

            List<Polyline> cutLines = elementToSplitCrv.SplitAtPoints(cuttingPts);

            if (cutLines.Count == 1) return new List<Panel> { panel };

            List<Panel> splitElements = new List<Panel>();

            foreach(Polyline pLine in cutLines)
            {
                if(!pLine.IsLinear())
                {
                    //Only do this for non-straight line cuts
                    List<Point> ctrlPts = pLine.IControlPoints();
                    ctrlPts.Add(ctrlPts[0]); //Close the polyline
                    Polyline completeCrv = BH.Engine.Geometry.Create.Polyline(ctrlPts);

                    Panel cpy = panel.Copy();
                    cpy.ExternalEdges = completeCrv.ToEdges();
                    cpy.CustomData = new Dictionary<string, object>(panel.CustomData);
                    splitElements.Add(cpy);
                }
            }

            return splitElements;
        }

        [Description("BH.Engine.Environment.Modify.Split => Returns a collection of Environment Panels that are split by their overlapping elements")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("A collection of Environment Panels that have been split")]
        public static List<Panel> Split(this List<Panel> panels)
        {
            //Go through all building elements and compare to see if any should be split into smaller building elements
            List<Panel> rtn = new List<Panel>();

            Dictionary<Panel, List<Panel>> overlaps = new Dictionary<Panel, List<Panel>>();

            foreach (Panel be in panels)
                overlaps.Add(be, be.IdentifyOverlaps(panels));

            foreach (KeyValuePair<Panel, List<Panel>> kvp in overlaps)
            {
                Dictionary<Panel, List<Panel>> rtn2 = new Dictionary<Panel, List<Panel>>();
                Dictionary<Panel, List<Polyline>> replacementGeom = new Dictionary<Panel, List<Polyline>>();
                Polyline be1P = kvp.Key.ToPolyline();

                foreach (Panel be2 in kvp.Value)
                {
                    Polyline be2p = be2.ToPolyline();

                    Dictionary<Panel, List<Polyline>> geomBuild = new Dictionary<Panel, List<Polyline>>();
                    geomBuild.Add(kvp.Key, new List<Polyline>());
                    geomBuild.Add(be2, new List<Polyline>());

                    List<Polyline> intersections = be1P.BooleanIntersection(be2p);
                    foreach(Polyline p in intersections)
                    {
                        geomBuild[kvp.Key].AddRange(be1P.SplitAtPoints(p.ControlPoints));
                        geomBuild[be2].AddRange(be2p.SplitAtPoints(p.ControlPoints));
                    }

                    foreach(KeyValuePair<Panel, List<Polyline>> kvp2 in geomBuild)
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

                    foreach (KeyValuePair<Panel, List<Polyline>> kvp3 in geomBuild)
                    {
                        foreach (Polyline p5 in kvp3.Value)
                        {
                            while (p5.ControlPoints.Last().Distance(p5.ControlPoints.First()) > 0.01)
                            {
                                bool addedPoint = false;
                                foreach (Point px in intersections[0].ControlPoints)
                                {
                                    if (!p5.ControlPoints.Contains(px) && !kvp3.Key.ToPolyline().ControlPoints.Contains(px) && px.MatchPointOn2Of3(p5.ControlPoints.Last()))
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
                            Panel newBE = ori.GetShallowClone() as Panel;
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