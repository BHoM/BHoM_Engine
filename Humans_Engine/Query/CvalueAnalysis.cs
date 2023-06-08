/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Humans.BodyParts;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluate Cvalues for a single Audience. See the wiki page to understand how Cvalue is calculated.")]
        [Input("audience", "Audience to evaluate.")]
        [Input("settings", "CvalueSettings to configure the evaluation.")]
        [Input("playingArea", "Polyline to be used for defining edge of performance or playing area.")]
        [Input("focalPoint", "Point defining a single focal point used by all spectators. Used only when CvalueFocalMethodEnum is SinglePoint.")]
        [Output("results", "Collection of Cvalue results.")]
        [DocumentationURL("https://bhom.xyz/documentation/Conventions/BHoM-View-quality-conventions/", oM.Base.Attributes.Enums.DocumentationType.Documentation)]
        public static List<Cvalue> CvalueAnalysis(this Audience audience, CvalueSettings settings, Polyline playingArea, Point focalPoint = null)
        {
            if (audience == null || settings == null || playingArea == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the CvalueAnalysis if the audience, settings, or playing area are null.");
                return new List<Cvalue>();
            }

            List<Cvalue> results = EvaluateCvalue(audience, settings, playingArea, focalPoint);
            return results;
        }

        /***************************************************/

        [Description("Evaluate Cvalues for a List of Audience. See the wiki page to understand how Cvalue is calculated.")]
        [Input("audience", "Audience to evaluate.")]
        [Input("settings", "CvalueSettings to configure the evaluation.")]
        [Input("playingArea", "Polyline to be used for defining edge of performance or playing area.")]
        [Input("focalPoint", "Point defining a single focal point used by all spectators. Used only when CvalueFocalMethodEnum is SinglePoint.")]
        [Output("results", "Collection of Cvalue results.")]
        [DocumentationURL("https://bhom.xyz/documentation/Conventions/BHoM-View-quality-conventions/", oM.Base.Attributes.Enums.DocumentationType.Documentation)]
        public static List<List<Cvalue>> CvalueAnalysis(this List<Audience> audience, CvalueSettings settings, Polyline playingArea, Point focalPoint = null)
        {
            if (audience == null || settings == null || playingArea == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the CvalueAnalysis if the audience, settings, or playing area are null.");
                return new List<List<Cvalue>>();
            }
            List<List<Cvalue>> results = new List<List<Cvalue>>();
            foreach (Audience a in audience)
            {
                results.Add(EvaluateCvalue(a, settings, playingArea, focalPoint));
            }
            return results;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Cvalue> EvaluateCvalue(Audience audience, CvalueSettings settings, Polyline playingArea, Point focalPoint = null)
        {
            List<Cvalue> results = new List<Cvalue>();
            if (audience.Spectators.Count == 0)
                return results;

            SetKDTree(audience);

            m_CvalueSettings = settings;
            m_FarClippingPlaneDistance = settings.FarClippingPlaneDistance;

            if (focalPoint == null)
                focalPoint = new Point();

            foreach (Spectator s in audience.Spectators)
            {

                m_CvalueExists = true;

                Point focal = GetFocalPoint(s, playingArea, focalPoint);
                double cvalue = GetCValue(s, focal);

                results.Add(CvalueResult(s, focal, cvalue));
            }
            return results;
        }

        /***************************************************/

        private static Point GetFocalPoint(Spectator spectator, Polyline playingArea, Point focalPoint)
        {
            Point focal = new Point();
            switch (m_CvalueSettings.FocalMethod)
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

                case CvalueFocalMethodEnum.SinglePoint:
                    focal = focalPoint;
                    break;
            }
            return focal;
        }

        /***************************************************/

        private static Cvalue CvalueResult(Spectator current, Point focal, double cvalue)
        {
            Vector d = current.Head.PairOfEyes.ReferenceLocation - focal;

            double absoluteDist = d.Length();
            double horizDist = Geometry.Create.Vector(d.X, d.Y, 0).Length();
            double heightAbovePitch = current.Head.PairOfEyes.ReferenceLocation.Z - focal.Z;

            double cValue;
            if (!m_CvalueExists)//
                cValue = m_CvalueSettings.DefaultCValue;
            else
                cValue = cvalue;

            return new Cvalue(current.BHoM_Guid, "", 0, cValue, horizDist, heightAbovePitch, absoluteDist, focal);

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
            foreach (Point ipt in interpts)
            {
                if (Geometry.Query.Distance(ipt, interPlane.Origin) < dist)
                {
                    focal = ipt;
                    dist = Geometry.Query.Distance(ipt, interPlane.Origin);
                }
            }
            return focal;
        }

        /***************************************************/

        private static Point FindFocalClosest(Spectator spect, Polyline focalPolyline)
        {
            return Geometry.Query.ClosestPoint(focalPolyline, spect.Head.PairOfEyes.ReferenceLocation);
        }

        /***************************************************/

        private static Point FindFocalOffset(Spectator spect, Polyline focalPolyline)
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

        private static double GetCValue(Spectator current, Point focalPoint)
        {
            //get spectators in front
            List<Spectator> infront = GetSpectatorsInfront(current, m_CvalueSettings.ViewConeAngle);
            if (infront.Count == 0)
            {
                m_CvalueExists = false;
                return 0;
            }

            Vector rowV = current.Head.PairOfEyes.ViewDirection.CrossProduct(Vector.ZAxis);
            //plane parallel to view direction perpendicular to row

            Plane viewVert = Geometry.Create.Plane(current.Head.PairOfEyes.ReferenceLocation, rowV.Normalise());
            double minDist = double.MaxValue;
            Point closest = null;
            foreach (Spectator s in infront)
            {
                Point proj = viewVert.ClosestPoint(s.Head.PairOfEyes.ReferenceLocation);
                double dist = proj.Distance(current.Head.PairOfEyes.ReferenceLocation);
                double d2 = s.Head.PairOfEyes.ReferenceLocation.Distance(current.Head.PairOfEyes.ReferenceLocation);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = proj;
                }
            }
            //check if closest in front is within clipping range
            if (minDist > m_CvalueSettings.FarClippingPlaneDistance)
            {
                m_CvalueExists = false;
                return 0;
            }

            Vector sightVect = Geometry.Create.Vector(current.Head.PairOfEyes.ReferenceLocation, focalPoint);
            Vector viewUp = sightVect.CrossProduct(rowV);
            Plane viewHoriz = Geometry.Create.Plane(current.Head.PairOfEyes.ReferenceLocation, viewUp);
            Line up = Geometry.Create.Line(closest, Vector.ZAxis);


            return closest.Distance(up.PlaneIntersection(viewHoriz, true));
        }

        /***************************************************/

        private static List<Spectator> GetSpectatorsInfront(Spectator current, double viewConeAngle)
        {
            PairOfEyes viewer = current.Head.PairOfEyes;

            double[] query = { viewer.ReferenceLocation.X, viewer.ReferenceLocation.Y, viewer.ReferenceLocation.Z };
            //first get the neighbourhood around the current spec
            var neighbours = m_KDTree.Nearest(query, radius: m_FarClippingPlaneDistance);

            List<Spectator> infront = new List<Spectator>();

            foreach (var n in neighbours)
            {
                PairOfEyes viewed = n.Node.Value.Head.PairOfEyes;
                Vector toNeighbour = viewed.ReferenceLocation - viewer.ReferenceLocation;
                toNeighbour.Z = 0;
                if (toNeighbour.Length() == 0)
                    continue;

                //point in plane within +-coneAngle in direction viewer is looking
                double testAngle = Geometry.Query.Angle(toNeighbour, viewer.ViewDirection);
                if (testAngle < viewConeAngle / 2)
                    infront.Add(n.Node.Value);
            }
            return infront;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool m_CvalueExists = true;
        private static CvalueSettings m_CvalueSettings;
        private static double m_FarClippingPlaneDistance;
    }
}

