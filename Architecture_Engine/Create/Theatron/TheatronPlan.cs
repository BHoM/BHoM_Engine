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
using BH.Engine.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** public Methods                            ****/
        /***************************************************/

        public static TheatronPlan PlanGeometry(List<Plane> structuralSections, ActivityArea activityArea,Polyline focalCurve)
        {
            var planGeometry = new TheatronPlan
            {
                SectionPlanes = structuralSections,
                ActivityArea = activityArea,
                FocalCurve = focalCurve,
            };
            setPlanes(ref planGeometry);
            return planGeometry;

        }
        /***************************************************/
        public static TheatronPlan PlanGeometry(StadiaParameters parameters)
        {
            //assuming its a full stadium
            var planGeometry = new TheatronPlan();
            
            switch (parameters.TypeOfBowl)
            {
                case StadiaType.Circular:
                    planGeometry = Compute.CircularPlan(parameters);
                    break;
                case StadiaType.NoCorners:
                    planGeometry = Compute.NoCornersPlan(parameters);
                    break;
                case StadiaType.Orthogonal:
                    planGeometry = Compute.OrthogonalPlan(parameters);
                    break;
                case StadiaType.EightArc:
                    planGeometry = Compute.EightArcPlan(parameters);
                    break;
                case StadiaType.FourArc:
                    planGeometry = Compute.FourArcPlan(parameters);
                    break;
            }
            planGeometry.ActivityArea = parameters.ActivityArea;
            planGeometry.FocalCurve = planGeometry.ActivityArea.PlayingArea;
            setPlanes(ref planGeometry);
            
            return planGeometry;

        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void setPlanes(ref TheatronPlan planGeometry)
        {
            planGeometry.TheatronFront = setFront(planGeometry.SectionPlanes);
            planGeometry.VomitoryPlanes = setVomitories(planGeometry.SectionPlanes);
            planGeometry.CombinedPlanes = combinedPlanes(planGeometry.SectionPlanes, planGeometry.VomitoryPlanes);
            findClosestSection(ref planGeometry);
            
        }
        private static List<Plane> setVomitories(List<Plane> sections)
        {
            List<Plane> vomitoryPlanes = new List<Plane>();
            double deltaX, deltaY, x, y;
            
            Vector nextUnitXdir = new Vector();
            Vector unitXdir = new Vector();
            //need the conditionals for partial bowl and no corners here.
            for (var i = 0; i < sections.Count-1; i++)
            {

                unitXdir = sections[i].Normal.CrossProduct(Vector.ZAxis);
                unitXdir.Normalise();
                
                nextUnitXdir = sections[i + 1].Normal.CrossProduct(Vector.ZAxis);
                nextUnitXdir.Normalise();
                deltaX = sections[i + 1].Origin.X - sections[i].Origin.X;
                deltaY = sections[i + 1].Origin.Y - sections[i].Origin.Y;

                x = (nextUnitXdir.X + unitXdir.X) / 2;
                y = (nextUnitXdir.Y + unitXdir.Y) / 2;
                
                Point origin = Geometry.Create.Point(sections[i].Origin.X + deltaX / 2, sections[i].Origin.Y + deltaY / 2, 0);
                Point xdir = Geometry.Create.Point(origin.X + x, origin.Y + y, 0);
                Point ydir = Geometry.Create.Point(origin.X + x, origin.Y + y, 1);
                vomitoryPlanes.Add(Geometry.Create.Plane(origin, xdir, ydir));
            }
            return vomitoryPlanes;
        }
        /***************************************************/
        private static List<Plane> combinedPlanes(List<Plane> sections, List<Plane> vomitories)
        {
            List<Plane> combined = new List<Plane>();
            for (int i = 0; i < sections.Count-1; i++)
            {
                combined.Add(sections[i]);
                combined.Add(vomitories[i]);
            }
            combined.Add(sections[sections.Count - 1]);
            return combined;
        }
        /***************************************************/
        private static void findClosestSection(ref TheatronPlan plan)
        {
            //this needs to be implmented on ICurve as the focal curve
            double shortestDist = double.PositiveInfinity;

            List<Point> allFocalPoints = new List<Point>();
            int index = 0;
            Point closestP = new Point();
            for (int i = 0; i < plan.SectionPlanes.Count; i++)
            {
                var p = Geometry.Query.ClosestPoint(plan.FocalCurve, plan.SectionPlanes[i].Origin);
                double dist = Geometry.Query.Distance(p, plan.SectionPlanes[i].Origin);
                if(dist< shortestDist)
                {
                    closestP = p;
                    shortestDist = dist;
                    index = i;
                }
            }
            plan.CValueFocalPoint = closestP;
            plan.MinDistToFocalCurve = shortestDist;
            plan.SectionClosestToFocalCurve = plan.SectionPlanes[index];

        }
        /***************************************************/
        private static Polyline setFront(List<Plane> sections)
        {
            List<Point> pts = sections.Select(item => item.Origin).ToList();
            Polyline front = Geometry.Create.Polyline(pts);
            return front;
        }
    }
}
