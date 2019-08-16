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
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** public Methods                            ****/
        /***************************************************/
        [Description("Create a partial TheatronPlan")]
        [Input("structuralSections", "List of ProfileOrigin to orientate the structural sections")]
        [Input("focalPolyline", "The polyline used to define focal points for Cvalue profile generation")]
        public static TheatronPlan PlanGeometry(List<ProfileOrigin> structuralSections, Polyline focalPolyline)
        {
            var planGeometry = new TheatronPlan
            {
                SectionOrigins = structuralSections,

                FocalCurve = focalPolyline,
            };
            planGeometry.SectionOrigins.ForEach(x => planGeometry.StructBayType.Add(BayType.Undefined));
            setPlanes(ref planGeometry);
            return planGeometry;

        }
        /***************************************************/
        [Description("Create a full TheatronPlan from StadiaParameters")]
        [Input("parameters", "StadiaParameters to define the TheatronPlan")]
        
        public static TheatronPlan PlanGeometry(StadiaParameters parameters)
        {
            //assuming its a full stadium
            TheatronPlan planGeometry = new TheatronPlan();
            
            switch (parameters.TypeOfBowl)
            {
                case StadiaType.Circular:
                    planGeometry = CircularPlan(parameters);
                    break;
                case StadiaType.NoCorners:
                    planGeometry = NoCornersPlan(parameters);
                    break;
                case StadiaType.Orthogonal:
                    planGeometry = OrthogonalPlan(parameters);
                    break;
                case StadiaType.EightArc:
                    planGeometry = EightArcPlan(parameters);
                    break;
                case StadiaType.FourArc:
                    planGeometry = FourArcPlan(parameters);
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
            planGeometry.TheatronFront = setFront(planGeometry.SectionOrigins);
            planGeometry.VomitoryOrigins = setVomitories(planGeometry.SectionOrigins);
            planGeometry.CombinedOrigins = combinedPlanes(planGeometry.SectionOrigins, planGeometry.VomitoryOrigins);
            if(planGeometry.FocalCurve!=null)findClosestSection(ref planGeometry);
            
        }
        private static List<ProfileOrigin> setVomitories(List<ProfileOrigin> sections)
        {
            List<ProfileOrigin> vomitoryOrigins = new List<ProfileOrigin>();
            double deltaX, deltaY, x, y;
            
            Vector nextUnitXdir = new Vector();
            Vector unitXdir = new Vector();
            //need the conditionals for partial bowl and no corners here.
            for (var i = 0; i < sections.Count-1; i++)
            {

                unitXdir = sections[i].Direction;
                unitXdir.Normalise();
                
                nextUnitXdir = sections[i + 1].Direction;
                nextUnitXdir.Normalise();
                deltaX = sections[i + 1].Origin.X - sections[i].Origin.X;
                deltaY = sections[i + 1].Origin.Y - sections[i].Origin.Y;

                x = (nextUnitXdir.X + unitXdir.X) / 2;
                y = (nextUnitXdir.Y + unitXdir.Y) / 2;
                
                Point origin = Geometry.Create.Point(sections[i].Origin.X + deltaX / 2, sections[i].Origin.Y + deltaY / 2, 0);
                Vector xdir = Geometry.Create.Vector(x, y, 0);
                
                vomitoryOrigins.Add(Create.ProfileOrigin(origin, xdir));
            }
            return vomitoryOrigins;
        }
        /***************************************************/
        private static List<ProfileOrigin> combinedPlanes(List<ProfileOrigin> sections, List<ProfileOrigin> vomitories)
        {
            List<ProfileOrigin> combined = new List<ProfileOrigin>();
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
            for (int i = 0; i < plan.VomitoryOrigins.Count; i++)
            {
                var p = Geometry.Query.ClosestPoint(plan.FocalCurve, plan.VomitoryOrigins[i].Origin);
                double dist = Geometry.Query.Distance(p, plan.VomitoryOrigins[i].Origin);
                if(dist< shortestDist)
                {
                    closestP = p;
                    shortestDist = dist;
                    index = i;
                }
            }
            plan.CValueFocalPoint = closestP;
            plan.MinDistToFocalCurve = shortestDist;
            plan.SectionClosestToFocalCurve = plan.VomitoryOrigins[index];

        }
        /***************************************************/
        private static Polyline setFront(List<ProfileOrigin> sections)
        {
            List<Point> pts = sections.Select(item => item.Origin).ToList();
            
            Polyline front = Geometry.Create.Polyline(pts);
            return front;
        }
        

    }
}
