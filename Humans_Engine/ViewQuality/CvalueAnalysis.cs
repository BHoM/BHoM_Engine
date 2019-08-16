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
using Accord.Collections;
using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Evaulate Cvalues for a single Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("settings", "CvalueSettings to configure the evaluation")]
        [Input("focalPolyline", "Polyline to be used for defining focal points")]
        public static List<Cvalue> CvalueAnalysis(Audience audience, CvalueSettings settings, Polyline focalPolyline)
        {
            List<Cvalue> results = EvaluateCvalue(audience, settings, focalPolyline);
            return results;
        }
        /***************************************************/
        [Description("Evaulate Cvalues for a List of Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("settings", "CvalueSettings to configure the evaluation")]
        [Input("focalPolyline", "Polyline to be used for defining focal points")]
        public static List<List<Cvalue>> CvalueAnalysis(List<Audience> audience, CvalueSettings settings, Polyline focalPolyline)
        {
            List<List<Cvalue>> results = new List<List<Cvalue>>();
            foreach(Audience a in audience)
            {
                results.Add(EvaluateCvalue(a, settings, focalPolyline));
            }
            return results;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<Cvalue> EvaluateCvalue(Audience audience, CvalueSettings settings, Polyline focalPolyline)
        {
            List<Cvalue> results = new List<Cvalue>();
            KDTree<Spectator> spectatorTree = SetKDTree(audience);
            foreach (Spectator s in audience.Spectators)
            {
                bool cvalueExists = true;
                Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, s.Head.PairOfEyes.ViewDirection);

                Spectator infront = GetSpecInfront(s, spectatorTree);
                double riserHeight = 0;
                double rowWidth = 0;
                Point focal = new Point();
                if (infront == null)
                {
                    //no spectator infront
                    cvalueExists = false;
                }
                else
                {
                    //check the infront and current are on parallel rows
                    if (infront.Head.PairOfEyes.ViewDirection != s.Head.PairOfEyes.ViewDirection)
                    {
                        cvalueExists = false;
                    }
                }
                if (cvalueExists)
                {
                    riserHeight = s.Head.PairOfEyes.ReferenceLocation.Z - infront.Head.PairOfEyes.ReferenceLocation.Z;
                    rowWidth = GetRowWidth(s, infront, rowVector);
                    focal = GetFocalPoint(rowVector, s, settings.FocalMethod, focalPolyline);
                }

                results.Add(CvalueResult(s, focal, riserHeight, rowWidth, cvalueExists, rowVector, settings));
            }
            return results;
        }
        /***************************************************/
        private static double GetRowWidth(Spectator current, Spectator nearest, Vector rowV)
        {
        double width = 0;
        Vector row = nearest.Head.PairOfEyes.ReferenceLocation - current.Head.PairOfEyes.ReferenceLocation;

        Vector row2d = Geometry.Create.Vector(row.X, row.Y, 0);//horiz vector to spectator row in front
        Vector projected = row2d.Project(rowV);
            
        Vector rowWidth = row2d - projected;
        width = rowWidth.Length();
        return width;
        }
        /***************************************************/
        private static Point GetFocalPoint(Vector rowV, Spectator spectator,CvalueFocalMethodEnum focalMethod,Polyline focalPolyline)
        {
            Point focal = new Point();
            switch (focalMethod)
            {
                case CvalueFocalMethodEnum.OffsetThroughCorners:
                    focal = FindFocalOffset(rowV, spectator,focalPolyline);

                    break;
                case CvalueFocalMethodEnum.Closest:
                    focal = FindFocalClosest(spectator, focalPolyline);

                    break;
                case CvalueFocalMethodEnum.Perpendicular:
                    focal = FindFocalPerp(rowV, spectator, focalPolyline);

                    break;
            }
            return focal;
        }
        /***************************************************/
        private static Cvalue CvalueResult(Spectator s, Point focal, double riser, double rowWidth, bool cvalueExists, Vector rowV,CvalueSettings settings)
        {
            Cvalue result = new Cvalue();
            Vector d = s.Head.PairOfEyes.ReferenceLocation - focal;
            result.AbsoluteDist = d.Length();
            result.Focalpoint = focal;
            result.HorizDist = Geometry.Create.Vector(d.X, d.Y, 0).Length();
            result.HeightAbovePitch = s.Head.PairOfEyes.ReferenceLocation.Z - focal.Z;
            
            if (!cvalueExists|| riser > settings.RowTolerance)//
            {
                result.CValue = settings.DefaultCValue;
            }
            else
            {
                result.CValue = (result.HorizDist - rowWidth) * (result.HeightAbovePitch / result.HorizDist) - (result.HeightAbovePitch - riser);
            }
            return result;

        }
        /***************************************************/
        private static Point FindFocalPerp(Vector rowV, Spectator spect, Polyline focalPolyline)
        {

            Point focal = new Point();
            //plane is perpendicular to row
            Plane interPlane = Geometry.Create.Plane(spect.Head.PairOfEyes.ReferenceLocation, rowV);
            double dist = Double.MaxValue;
            Point ipt = new Point();
            //loop the segments in the focalPolyline find the closest perpendicular point
            foreach(var seg in focalPolyline.SubParts())
            {
                seg.Infinite = true;
                ipt = Geometry.Query.PlaneIntersection(seg, interPlane);
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
        private static Point FindFocalOffset(Vector rowVector, Spectator spect, Polyline focalPolyline)
        {
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
        private static Spectator GetSpecInfront(Spectator current, KDTree<Spectator> tree)
        {
            
            double[] query = { current.Head.PairOfEyes.ReferenceLocation.X, current.Head.PairOfEyes.ReferenceLocation.Y, current.Head.PairOfEyes.ReferenceLocation.Z };
            //first get the neighbourhood around the current spec
            var neighbours = tree.Nearest(query, neighbors: 8);
            
            NodeDistance<KDTreeNode<Spectator>> closestInFront = new NodeDistance<KDTreeNode<Spectator>>();
            double dist = Double.MaxValue;
            foreach (var n in neighbours)
            {
                //only those infront
                if (n.Node.Value.Head.PairOfEyes.ReferenceLocation.Z < current.Head.PairOfEyes.ReferenceLocation.Z)
                {
                    if (n.Distance < dist)
                    {
                        closestInFront = n;
                        dist = n.Distance;
                    }
                }
            }
            if (closestInFront.Node == null) return null;
            else return closestInFront.Node.Value;
        }
    }
}
