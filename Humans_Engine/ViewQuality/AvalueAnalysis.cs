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
using BH.Engine.Architecture.Theatron;
using System.Linq;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static List<Avalue> AvalueAnalysis(List<Audience> fullAudience, AvalueSettings settings, ActivityArea activityArea)
        {
            List<Avalue> results = new List<Avalue>();
            foreach (Audience audience in fullAudience)
            {
                foreach (Spectator s in audience.Spectators)
                {
                    Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, s.Eye.ViewDirection);
                    Vector viewVect = activityArea.AValueFocalPoint - s.Eye.Location;
                    results.Add(ClipView(s, rowVector, viewVect, settings,activityArea));
                }
            }
            return results;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Avalue ClipView(Spectator origin, Vector rowV, Vector viewVect, AvalueSettings settings, ActivityArea activityArea)
        {
            Avalue result = new Avalue();
            
            Vector viewY = Geometry.Query.CrossProduct(viewVect, rowV);
            Vector viewX = Geometry.Query.CrossProduct(viewVect, viewY);
            viewY.Reverse();
            viewX.Reverse();
            viewVect = viewVect.Normalise();
            Vector shift = viewVect * settings.EyeFrameDist;

            Point shiftOrigin = origin.Eye.Location + shift;
            Point viewClipOrigin = origin.Eye.Location + 2 * shift;

            //planes need orientation
            Plane viewPlane = Geometry.Create.Plane(shiftOrigin, viewVect);
            Plane viewClip = Geometry.Create.Plane(viewClipOrigin, viewVect);
            //get the view cone
            result.ViewCone = Create.ViewCone(viewY, viewX, shiftOrigin, 1, settings.ConeType);
            //find part of activity area to project
            Polyline clippedArea = ClipActivityArea(viewClip, activityArea);
            //project the pitch
            result.FullActivityArea = ProjectPolylineToPlane(viewPlane, clippedArea, origin.Eye.Location);
            //clip the pitch against the viewcone
            result.ClippedActivityArea = clipActivity(result.FullActivityArea, result.ViewCone, origin, viewPlane);
            //calculate the avalue
            result.AValue = result.ClippedActivityArea.Sum(x=>x.Area())/result.ViewCone.ConeArea*100;
            //clipp heads infront


            return result;
        }

        /***************************************************/

        private static Polyline ClipActivityArea(Plane clipping, ActivityArea activityArea)
        {
            List<Point> control = new List<Point>();
            foreach(Line seg in activityArea.PlayingArea.SubParts())
            {
                Point p = seg.PlaneIntersection(clipping, false);
                if (p == null)
                {
                    //is start infront or behind plane?
                    Point s = seg.StartPoint();
                    Point e = clipping.Origin;
                    Vector v = Geometry.Create.Vector( e ) - Geometry.Create.Vector(s);
                    double d = Geometry.Query.DotProduct(clipping.Normal, v);
                    if(d<0)//infront
                    {
                        control.Add(s);
                    }
                }
                else
                {
                    control.Add(p);
                }
            }
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
                Point interPt = test.PlaneIntersection(plane, false);
                if (interPt != null)
                {
                    control.Add(interPt);
                }
            }
            Polyline projected = Geometry.Create.Polyline(control);
            return projected;
        }

        /***************************************************/

        private static List<Polyline> clipActivity(Polyline projected, ViewCone viewCone, Spectator origin, Plane viewPlane)
        {
            List<Polyline> clippedArea = new List<Polyline>();
            for (int i = 0; i < viewCone.ConeBoundary.Count; i++)
            {
                
                Polyline temp = Architecture.Theatron.Compute.SutherlandHodgman(projected, viewCone.ConeBoundary[i]);

                clippedArea.Add(temp);
            }
            return clippedArea;
        }

    }
}
