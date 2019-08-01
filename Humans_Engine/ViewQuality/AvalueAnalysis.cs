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

using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Architecture.Theatron;
using BH.oM.Humans.ViewQuality;
using BH.Engine.Geometry;
using System.Linq;

using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Collections;
using System.IO;
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Evaulate Avalues for a single Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("setting", "AvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<Avalue> AvalueAnalysis(Audience audience, AvalueSettings settings, ActivityArea activityArea)
        {
            List<Avalue> results = EvaluateAvalue(audience, settings, activityArea);
            return results;
        }
        /***************************************************/
        [Description("Evaulate Avalues for a List of Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("setting", "AvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<List<Avalue>> AvalueAnalysis(List<Audience> audience, AvalueSettings settings, ActivityArea activityArea)
        {
            List<List<Avalue>> results = new List<List<Avalue>>();
            foreach(Audience a in audience)
            {
                results.Add(EvaluateAvalue(a, settings, activityArea));
            }
            return results;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<Avalue> EvaluateAvalue(Audience audience, AvalueSettings settings, ActivityArea activityArea)
        {
            List<Avalue> results = new List<Avalue>();
            KDTree<Spectator> spectatorTree = null;
            if (settings.CalculateOcclusion) spectatorTree = SetKDTree(audience);

            foreach (Spectator s in audience.Spectators)
            {
                Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, s.Head.PairOfEyes.ViewDirection);
                Vector viewVect = activityArea.ActivityFocalPoint - s.Head.PairOfEyes.ReferenceLocation;
                results.Add(ClipView(s, rowVector, viewVect, settings, activityArea, spectatorTree));
            }
            return results;
        }
        /***************************************************/
        private static KDTree<Spectator> SetKDTree(Audience audience)
        {
            List<double[]> points = new List<double[]>();

            foreach (Spectator s in audience.Spectators)
            {
                double[] pt = new double[] { s.Head.PairOfEyes.ReferenceLocation.X, s.Head.PairOfEyes.ReferenceLocation.Y, s.Head.PairOfEyes.ReferenceLocation.Z };
                points.Add(pt);
            }
            // To create a tree from a set of points, we use
            KDTree<Spectator> tree = KDTree.FromData<Spectator>(points.ToArray(),audience.Spectators.ToArray(),true);

            return tree;
        }
        
        /***************************************************/

        private static Avalue ClipView(Spectator spectator, Vector rowV, Vector viewVect, AvalueSettings settings, ActivityArea activityArea, KDTree<Spectator> tree)
        {
            Avalue result = new Avalue();
            
            Vector viewY = Geometry.Query.CrossProduct(viewVect, rowV);
            Vector viewX = Geometry.Query.CrossProduct(viewVect, viewY);
            
            viewVect = viewVect.Normalise();
            Vector shift = viewVect * settings.EyeFrameDist;

            Point shiftOrigin = spectator.Head.PairOfEyes.ReferenceLocation + shift;
            Point viewClipOrigin = spectator.Head.PairOfEyes.ReferenceLocation + 2 * shift;

            //planes need orientation
            Plane viewPlane = Geometry.Create.Plane(shiftOrigin, viewVect);
            Plane viewClip = Geometry.Create.Plane(viewClipOrigin, viewVect);
            //get the view cone
            result.ViewCone = Create.ViewCone(viewY, viewX, shiftOrigin, 1, settings.ConeType);
            //find part of activity area to project
            Polyline clippedArea = ClipActivityArea(viewClip, activityArea);
            //project the pitch
            result.FullActivityArea = ProjectPolylineToPlane(viewPlane, clippedArea, spectator.Head.PairOfEyes.ReferenceLocation);
            //clip the pitch against the viewcone
            
            result.ClippedActivityArea = ClipActivityArea(result.FullActivityArea, result.ViewCone, spectator, viewPlane);
            //calculate the avalue
            result.AValue = result.ClippedActivityArea.Sum(x=>x.Area())/result.ViewCone.ConeArea*100;
            //clip heads infront
            if (settings.CalculateOcclusion)
            {
                List<Spectator> occludingSpectators = GetPotentialOcclusion(spectator, tree, 3);
                if (occludingSpectators.Count > 0)
                {
                    
                    List <Polyline> occludingClippedHeads = ClipHeads(occludingSpectators, spectator, viewPlane, result.ClippedActivityArea);
                    if (occludingClippedHeads.Count > 0)
                    {
                        result.Heads = occludingClippedHeads;
                        result.Occulsion = occludingClippedHeads.Sum(x => x.Area()) / result.ViewCone.ConeArea * 100;
                        CheckPolylineGeo(occludingClippedHeads);
                    }
                    
                }
                
                
            }

            return result;
        }
        /***************************************************/
        private static void CheckPolylineGeo(List<Polyline> tests)
        {
            StreamWriter sw = new StreamWriter("geodump.txt");
            foreach(Polyline pl in tests)
            {
                foreach(Point p in pl.ControlPoints)
                {
                    sw.Write(string.Format("{0},{1},{2},",p.X,p.Y,p.Z));
                }
                sw.WriteLine("");
            }
            sw.WriteLine("");
            sw.Close();
        }
        /***************************************************/

        private static List<Spectator> GetPotentialOcclusion(Spectator spectator,KDTree<Spectator> tree,double neighborhoodRadius)
        {
            double[] query = new double[] { spectator.Head.PairOfEyes.ReferenceLocation.X, spectator.Head.PairOfEyes.ReferenceLocation.Y, spectator.Head.PairOfEyes.ReferenceLocation.Z };
            var neighbours = tree.Nearest(query, radius: neighborhoodRadius);
            List<Spectator> infront = new List<Spectator>();
            foreach(var n in neighbours)
            {
                var nodeZ = n.Node.Position[2];
                if (nodeZ < spectator.Head.PairOfEyes.ReferenceLocation.Z)
                {
                    infront.Add(n.Node.Value);
                }
            }
            infront.ForEach(x => x.HeadOutline = Create.GetHeadOutline(x.Head,3));

            return infront;
        }

        /***************************************************/
        private static List<Polyline> ClipHeads(List<Spectator> nearSpecs, Spectator current, Plane viewPlane, List<Polyline> ClippedArea)
        {
            //using the project activity area to clip heads infront that over lap
            List<Polyline> clippedHeads = new List<Polyline>();
            foreach (Spectator s in nearSpecs)
            {
                foreach (Polyline pl in ClippedArea)
                {
                    Polyline projectedHead = ProjectPolylineToPlane(viewPlane, s.HeadOutline, current.Head.PairOfEyes.ReferenceLocation);

                    Polyline subject = projectedHead.DeepClone();
                    int np = subject.ControlPoints.Count;
                    Polyline clippedHead = Geometry.Compute.ClipPolylines(subject, pl);
                    if (clippedHead.Area() > 0)
                    {
                        clippedHeads.Add(clippedHead);
                    }
                    
                }
            }
            return clippedHeads;
        }
        /***************************************************/

        private static Polyline ClipActivityArea(Plane clipping, ActivityArea activityArea)
        {
            List<Point> control = new List<Point>();
            foreach(Line seg in activityArea.PlayingArea.SubParts())
            {
                //is start infront or behind plane?
                Point s = seg.StartPoint();
                Point e = clipping.Origin;
                Vector v = Geometry.Create.Vector(e) - Geometry.Create.Vector(s);
                double d = Geometry.Query.DotProduct(clipping.Normal, v);
                if (d < 0)//infront
                {
                    control.Add(s);
                }
                Point p = seg.PlaneIntersection(clipping, false);
                if (p != null) control.Add(p);
            }
            //close the polyline
            control.Add(control[0]);
            Polyline clippedPoly = Geometry.Create.Polyline(control);
            return clippedPoly;
        }

        /***************************************************/

        private static Polyline ProjectPolylineToPlane(Plane plane,Polyline polyline,Point viewPoint)
        {
            List<Point> control = new List<Point>();
            foreach (Point p in polyline.ControlPoints)
            {
                Line test = Geometry.Create.Line(viewPoint, p);
                Point interPt = test.PlaneIntersection(plane, true);
                if (interPt != null)
                {
                    control.Add(interPt);
                }
            }
            Polyline projected = Geometry.Create.Polyline(control);
            return projected;
        }

        /***************************************************/

        private static List<Polyline> ClipActivityArea(Polyline projected, ViewCone viewCone, Spectator origin, Plane viewPlane)
        {
            List<Polyline> clippedArea = new List<Polyline>();
            for (int i = 0; i < viewCone.ConeBoundary.Count; i++)
            {
                var subject = projected.DeepClone();
                Polyline temp = Geometry.Compute.ClipPolylines(subject, viewCone.ConeBoundary[i]);

                clippedArea.Add(temp);
            }
            return clippedArea;
        }
        
        /***************************************************/
        
    }
}
