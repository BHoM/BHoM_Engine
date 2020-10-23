/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using Accord.Collections;
using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Humans.BodyParts;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Evaluate Cvalues for a single Audience")]
        [Input("audience", "Audience to evaluate")]
        [Input("settings", "CvalueSettings to configure the evaluation")]
        [Input("playingArea", "Polyline to be used for defining edge of performance or playing area")]
        [Input("focalPoint", "Point defining the spectator focal point")]
        public static List<Cvalue> CvalueAnalysis(Audience audience, CvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<Cvalue> results = EvaluateCvalue(audience, settings, playingArea, focalPoint);
            return results;
        }
        /***************************************************/
        [Description("Evaluate Cvalues for a List of Audience")]
        [Input("audience", "Audience to evaluate")]
        [Input("settings", "CvalueSettings to configure the evaluation")]
        [Input("playingArea", "Polyline to be used for defining edge of performance or playing area")]
        [Input("focalPoint", "Point defining the spectator focal point")]
        public static List<List<Cvalue>> CvalueAnalysis(List<Audience> audience, CvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<List<Cvalue>> results = new List<List<Cvalue>>();
            foreach(Audience a in audience)
            {
                results.Add(EvaluateCvalue(a, settings, playingArea, focalPoint));
            }
            return results;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<Cvalue> EvaluateCvalue(Audience audience, CvalueSettings settings, Polyline playingArea, Point focalPoint)
        {
            List<Cvalue> results = new List<Cvalue>();
            if (audience.Spectators.Count == 0)
                return results;
            KDTree<Spectator> spectatorTree = SetKDTree(audience);
            foreach (Spectator s in audience.Spectators)
            {
                bool cvalueExists = true;

                Point focal = GetFocalPoint(s, settings.FocalMethod, playingArea, focalPoint);
                Spectator infront = GetSpecInfront(s, spectatorTree, focal);
                if (infront == null)
                {
                    //no spectator in front
                    cvalueExists = false;
                }
                //results.Add(CvalueResult(s, focal, riserHeight, rowWidth, cvalueExists, rowVector, settings));
                results.Add(CvalueResult(infront, s, focal, cvalueExists, settings));
            }
            return results;
        }
        
        /***************************************************/
        private static Point GetFocalPoint(Spectator spectator,CvalueFocalMethodEnum focalMethod, Polyline playingArea, Point focalPoint)
        {
            Point focal = new Point();
            switch (focalMethod)
            {
                case CvalueFocalMethodEnum.OffsetThroughCorners:
                    focal = FindFocalOffset(spectator, playingArea);

                    break;
                case CvalueFocalMethodEnum.Closest:
                    focal = FindFocalClosest(spectator, playingArea);

                    break;
                case CvalueFocalMethodEnum.Perpendicular:
                    focal = FindFocalPerp(spectator, playingArea);

                    break;
                case CvalueFocalMethodEnum.ActivityFocalPoint:
                    focal = focalPoint;

                    break;
            }
            return focal;
        }
        
        /***************************************************/
        private static Cvalue CvalueResult(Spectator infront, Spectator current, Point focal,  bool cvalueExists,  CvalueSettings settings)
        {
            Cvalue result = new Cvalue();
            result.ObjectId = current.BHoM_Guid;
            Vector d = current.Head.PairOfEyes.ReferenceLocation - focal;
            Line sightline = Geometry.Create.Line(current.Head.PairOfEyes.ReferenceLocation, d);
            result.AbsoluteDist = d.Length();
            result.Focalpoint = focal;
            result.HorizDist = Geometry.Create.Vector(d.X, d.Y, 0).Length();
            result.HeightAbovePitch = current.Head.PairOfEyes.ReferenceLocation.Z - focal.Z;

            if (!cvalueExists)//
            {
                result.CValue = settings.DefaultCValue;
            }
            else
            {
                Plane vertical = Geometry.Create.Plane(infront.Head.PairOfEyes.ReferenceLocation, infront.Head.PairOfEyes.ViewDirection);
                Plane horizontal = Geometry.Create.Plane(infront.Head.PairOfEyes.ReferenceLocation, Vector.ZAxis);
                Point p = Geometry.Query.PlaneIntersection(sightline, vertical);
                result.CValue = p.Distance(horizontal.ClosestPoint(p));
            }
            return result;

        }
        /***************************************************/
        private static Point FindFocalPerp(Spectator spect, Polyline focalPolyline)
        {
            Vector rowV = Geometry.Query.CrossProduct(Vector.ZAxis, spect.Head.PairOfEyes.ViewDirection);
            Point focal = new Point();
            //plane is perpendicular to row
            Plane interPlane = Geometry.Create.Plane(spect.Head.PairOfEyes.ReferenceLocation, rowV);
            double dist = Double.MaxValue;
            //loop the segments in the focalPolyline find the closest perpendicular point
            List<Point> interpts = focalPolyline.IPlaneIntersections(interPlane);
            foreach(Point ipt in interpts)
            {
                if (Geometry.Query.Distance(ipt, interPlane.Origin) < dist)
                {
                    focal = ipt;
                    dist = Geometry.Query.Distance(ipt, interPlane.Origin);
                }
            }
            return focal;
        }
        private static Point FindFocalClosest(Spectator spect, Polyline focalPolyline)
        {
            return Geometry.Query.ClosestPoint(focalPolyline, spect.Head.PairOfEyes.ReferenceLocation);
        }
        private static Point FindFocalOffset( Spectator spect, Polyline focalPolyline)
        {
            Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, spect.Head.PairOfEyes.ViewDirection);
            Point focal = new Point();
            //plane is perpendicular to row
            Plane interPlane = Geometry.Create.Plane(spect.Head.PairOfEyes.ReferenceLocation, rowVector);
            double dist = Double.MaxValue;
            Point ipt = new Point();
            //loop the segments in the focalPolyline 
            //from the start point create a line parallel to the row
            //inter sect with the plane 
            foreach (var seg in focalPolyline.SubParts())
            {
                Line offset = Geometry.Create.Line(seg.StartPoint(), seg.StartPoint() + rowVector);
                offset.Infinite = true;
                ipt = Geometry.Query.PlaneIntersection(offset, interPlane);
                if (Geometry.Query.Distance(ipt, interPlane.Origin) < dist)
                {
                    focal = ipt;
                    dist = Geometry.Query.Distance(ipt, interPlane.Origin);
                }
            }
            return focal;
            
        }
        /***************************************************/
        private static Spectator GetSpecInfront(Spectator current, KDTree<Spectator> tree,Point focalPoint)
        {
            PairOfEyes viewer = current.Head.PairOfEyes;
            double[] query = { viewer.ReferenceLocation.X, viewer.ReferenceLocation.Y, viewer.ReferenceLocation.Z };
            //first get the neighbourhood around the current spec
            var neighbours = tree.Nearest(query, neighbors: 8);
            
            NodeDistance<KDTreeNode<Spectator>> closestInFront = new NodeDistance<KDTreeNode<Spectator>>();
            
            double minDist = double.MaxValue;
            foreach (var n in neighbours)
            {
                PairOfEyes viewed = n.Node.Value.Head.PairOfEyes;
                Vector toNeighbour = viewed.ReferenceLocation- viewer.ReferenceLocation ;
                if (toNeighbour.Length() == 0)
                    continue;
                //closest point within +-60 degrees in direction viewer is looking
                double testAngle = Geometry.Query.Angle(toNeighbour, viewer.ViewDirection);
                if (testAngle < 1.0472)
                {
                    if(viewed.ReferenceLocation.Distance(viewer.ReferenceLocation) < minDist)
                    {
                        minDist = viewed.ReferenceLocation.Distance(viewer.ReferenceLocation);
                        closestInFront = n;
                    }
                }
            }
            if (closestInFront.Node == null) return null;
            else return closestInFront.Node.Value;
        }
    }
}

