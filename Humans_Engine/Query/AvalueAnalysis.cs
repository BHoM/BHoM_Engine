/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Humans.ViewQuality;
using BH.Engine.Geometry;
using System.Linq;
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
        
        [PreviousVersion("4.2", "BH.Engine.Humans.ViewQuality.Query.AvalueAnalysis(BH.oM.Humans.ViewQuality.Audience, BH.oM.Humans.ViewQualityAvalueSettings, BH.oM.Architecture.Theatron.ActivityArea)")]
        [Description("Evaluate Avalues for a single Audience")]
        [Input("audience", "Audience to evaluate")]
        [Input("settings", "AvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<Avalue> AvalueAnalysis(Audience audience, AvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<Avalue> results = EvaluateAvalue(audience, settings, playingArea, focalPoint);
            return results;
        }

        /***************************************************/

        [PreviousVersion("4.2", "BH.Engine.Humans.ViewQuality.Query.AvalueAnalysis(System.Collections.Generic.List<BH.oM.Humans.ViewQuality.Audience>, BH.oM.Humans.ViewQualityAvalueSettings, BH.oM.Architecture.Theatron.ActivityArea)")]
        [Description("Evaluate Avalues for a List of Audience")]
        [Input("audience", "Audience to evaluate")]
        [Input("settings", "AvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<List<Avalue>> AvalueAnalysis(List<Audience> audience, AvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<List<Avalue>> results = new List<List<Avalue>>();
            foreach(Audience a in audience)
            {
                results.Add(EvaluateAvalue(a, settings, playingArea, focalPoint));
            }
            return results;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<Avalue> EvaluateAvalue(Audience audience, AvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<Avalue> results = new List<Avalue>();
            KDTree<Spectator> spectatorTree = null;
            m_AvalueSettings = settings;

            if (settings.CalculateOcclusion) spectatorTree = SetKDTree(audience);

            foreach (Spectator s in audience.Spectators)
            {
                Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, s.Head.PairOfEyes.ViewDirection);
                Vector viewVect = focalPoint - s.Head.PairOfEyes.ReferenceLocation;
                results.Add(ClipView(s, rowVector, viewVect, playingArea, spectatorTree));
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

        private static Avalue ClipView(Spectator spectator, Vector rowV, Vector viewVect, Polyline activityArea, KDTree<Spectator> tree)
        {
            Avalue result = new Avalue();
            
            Vector viewY = Geometry.Query.CrossProduct(viewVect, rowV);
            Vector viewX = Geometry.Query.CrossProduct(viewVect, viewY);
            viewX = viewX.Normalise();
            viewY = viewY.Normalise();

            viewVect = viewVect.Normalise();
            Vector shift = viewVect * m_AvalueSettings.EyeFrameDist;

            Point shiftOrigin = spectator.Head.PairOfEyes.ReferenceLocation + shift;
            Point viewClipOrigin = spectator.Head.PairOfEyes.ReferenceLocation + 2 * shift;

            //planes need orientation
            Plane viewPlane = Geometry.Create.Plane(shiftOrigin, viewVect);
            Plane viewClip = Geometry.Create.Plane(viewClipOrigin, viewVect);

            //get the view cone
            result.ViewCone = m_AvalueSettings.EffectiveConeOfVision;

            //find part of activity area to project
            Polyline clippedArea = ReduceActivityArea(viewClip, activityArea);

            //project the pitch
            result.FullActivityArea = ProjectPolylineToPlane(viewPlane, clippedArea, spectator.Head.PairOfEyes.ReferenceLocation);

            //clip the projected pitch against the view cone
            result.ClippedActivityArea = ClipActivityArea(result.FullActivityArea, result.ViewCone);

            //calculate the avalue
            result.AValue = result.ClippedActivityArea.Area() / result.ViewCone.Area() * 100;

            //clip heads in front
            if (m_AvalueSettings.CalculateOcclusion)
            {
                List<Spectator> infront = GetSpectatorsInfront(spectator, tree, m_AvalueSettings.ViewConeAngle);
                if (infront.Count > 0)
                {
                    
                    List <Polyline> occludingClippedHeads = ClipHeads(infront, spectator, viewPlane, result.ClippedActivityArea);
                    if (occludingClippedHeads.Count > 0)
                    {
                        result.Heads = occludingClippedHeads;
                        result.Occulsion = occludingClippedHeads.Sum(x => x.Area()) / result.ViewCone.Area() * 100;
                    }
                    
                }
                
            }

            return result;
        }

        /***************************************************/

        private static List<Polyline> ClipHeads(List<Spectator> nearSpecs, Spectator current, Plane viewPlane, Polyline clippedArea)
        {
            //using the project activity area to clip heads in front that over lap
            List<Polyline> clippedHeads = new List<Polyline>();
            foreach (Spectator s in nearSpecs)
            {
                Point p = viewPlane.ClosestPoint(s.Head.PairOfEyes.ReferenceLocation);
                //if distance to view plane is in range
                if (s.Head.PairOfEyes.ReferenceLocation.Distance(p) <= m_AvalueSettings.FarClippingPlaneDistance)
                {
                    if (s.HeadOutline.ControlPoints.Count == 0)
                        s.HeadOutline = Create.GetHeadOutline(s.Head);

                    Polyline projectedHead = ProjectPolylineToPlane(viewPlane, s.HeadOutline, current.Head.PairOfEyes.ReferenceLocation);

                    List<Polyline> clippedHead = Geometry.Compute.BooleanIntersection(clippedArea, projectedHead);
                    if (clippedHead.Count > 0)
                    {
                        clippedHeads.Add(clippedHead[0]);
                    }
                }
                
            }
            return clippedHeads;
        }

        /***************************************************/

        private static Polyline ReduceActivityArea(Plane clipping, Polyline activityArea)
        {
            //just the part in front of the spectator
            List<Point> control = new List<Point>();
            foreach(Line seg in activityArea.SubParts())
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

        private static Polyline ProjectPolylineToPlane(Plane plane, Polyline polyline, Point viewPoint)
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

        private static Polyline ClipActivityArea(Polyline projected, Polyline viewCone)
        {
            List<Polyline> temp = Geometry.Compute.BooleanIntersection(viewCone, projected);
            if (temp.Count > 0)
                return temp[0];
            else
                return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static AvalueSettings m_AvalueSettings;

    }
}


