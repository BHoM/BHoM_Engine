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

using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Architecture.Theatron;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static TheatronPlan EightArcPlan(StadiaParameters parameters)
        {
            TheatronPlan plan = new TheatronPlan();
            radialPlanSetUp(ref plan,parameters.PitchLength,parameters.PitchWidth, parameters.SideBound, parameters.SideRadius,
                parameters.EndBound, parameters.EndRadius, parameters.CornerRadius, parameters.NumCornerBays, parameters.StructBayWidth,
                parameters.CornerFraction);
            return plan;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void radialPlanSetUp(ref TheatronPlan plan,double length, double width, double sideBound, double sideRadius, double endBound, double endRadius, double cornerR, int nCornerBays, double structBayW, double cornerFraction)
        {
            plan.SectionOrigins = new List<ProfileOrigin>();
            
            int count = 0;
            double sidecentreX = width / 2 + sideBound - sideRadius;
            double sidecentreY = 0;
            double endcentreX = 0;
            double endcentreY = length / 2 + endBound - endRadius;
            Point intersect = intersectCircles(sidecentreX, sidecentreY, sideRadius - cornerR, endcentreX, endcentreY, endRadius - cornerR);
            double centreX = intersect.X;
            double centreY = intersect.Y;

            double sweepAngleSide = 2 * Math.Atan(centreY / (centreX - sidecentreX));
            double sweepSideBay = 2 * Math.Asin(structBayW / 2 / sideRadius);

            int nSideBays = (int)(Math.Floor(sweepAngleSide / sweepSideBay));

            double sweepAngleEnd = 2 * Math.Atan(centreX / (centreY - endcentreY));
            double sweepEndBay = 2 * Math.Asin(structBayW / 2 / endRadius);
            int nEndBays = (int)(Math.Floor(sweepAngleEnd / sweepEndBay));


            Vector endArc = Geometry.Create.Vector(centreX, centreY - endcentreY, 0);
            Vector sideArc = Geometry.Create.Vector(centreX - sidecentreX, centreY, 0);

            double cornerSweep = Geometry.Query.Angle(sideArc, endArc);//*Math.PI/180;
            double cornA = cornerSweep / (nCornerBays + 2 * cornerFraction);
            double trueR = cornerR / Math.Cos(cornA / 2);
            double theta = 1, startAngle = 1, radius = 1;
            int numBays = 1;
            bool fractionbayStart = false;
            BayType bayType = BayType.Side; //0 = side, 1= end, 2 =corner

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0://side right
                        centreX = sidecentreX;
                        centreY = 0;
                        theta = sweepSideBay;
                        numBays = nSideBays;
                        startAngle = (numBays * theta) / -2;
                        radius = sideRadius;
                        fractionbayStart = false;
                        bayType = BayType.Side;
                        break;
                    case 1://top right corner
                        centreX = intersect.X;
                        centreY = intersect.Y;
                        theta = cornA;
                        startAngle = sweepAngleSide / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = BayType.Corner;
                        break;
                    case 2://north end
                        centreX = 0;
                        centreY = endcentreY;
                        theta = sweepEndBay;
                        numBays = nEndBays;
                        startAngle = Math.PI / 2 - numBays * theta / 2;
                        radius = endRadius;
                        fractionbayStart = false;
                        bayType = BayType.End;
                        break;
                    case 3://north west corner
                        centreX = -intersect.X;
                        centreY = intersect.Y;
                        theta = cornA;
                        startAngle = Math.PI / 2 + sweepAngleEnd / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = BayType.Corner;
                        break;
                    case 4://west side
                        centreX = -sidecentreX;
                        centreY = 0;
                        theta = sweepSideBay;
                        numBays = nSideBays;
                        startAngle = Math.PI - (numBays * theta) / 2;
                        radius = sideRadius;
                        fractionbayStart = false;
                        bayType = BayType.Side;
                        break;
                    case 5://south west conrer
                        centreX = -intersect.X;
                        centreY = -intersect.Y;
                        theta = cornA;
                        startAngle = Math.PI + sweepAngleSide / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = BayType.Corner;
                        break;
                    case 6:// south end
                        centreX = 0;
                        centreY = -endcentreY;
                        theta = sweepEndBay;
                        numBays = nEndBays;
                        startAngle = 1.5 * Math.PI - numBays * theta / 2;
                        radius = endRadius;
                        fractionbayStart = false;
                        bayType = BayType.End;
                        break;
                    case 7://south east corner
                        centreX = intersect.X;
                        centreY = -intersect.Y;
                        theta = cornA;
                        startAngle = 1.5 * Math.PI + sweepAngleEnd / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = BayType.Corner;
                        break;
                }
                List<ProfileOrigin> partPlanes = arcSweepBay(centreX, centreY, theta, startAngle, radius, numBays, fractionbayStart, cornerFraction);
                for (int p = 0; p < partPlanes.Count; p++)
                {
                    plan.SectionOrigins.Add(partPlanes[p]);
                    
                    plan.StructBayType.Add(bayType);
                    count++;
                }
            }


        }
        /***************************************************/
        private static List<ProfileOrigin> arcSweepBay(double centreX, double centreY, double theta, double startAngle, double radius, int numBays, bool fractionbayStart, double fraction)
        {
            List<ProfileOrigin> sectionPlane = new List<ProfileOrigin>();
            double xO, yO, xP, yP;
            for (int d = 0; d <= numBays; d++)
            {

                //                                            
                if (fractionbayStart)//half bay on first for corners
                {
                    double fractionBay = theta * fraction;
                    if (d == 0)//half bay on first
                    {
                        xO = centreX + radius * Math.Cos(startAngle + fractionBay);
                        yO = centreY + radius * Math.Sin(startAngle + fractionBay);
                        xP = radius * Math.Cos(startAngle + fractionBay);
                        yP = radius * Math.Sin(startAngle + fractionBay);

                    }
                    else
                    {
                        xO = centreX + radius * Math.Cos(startAngle + (theta * d + fractionBay));
                        yO = centreY + radius * Math.Sin(startAngle + (theta * d + fractionBay));
                        xP = radius * Math.Cos(startAngle + (theta * d + fractionBay));
                        yP = radius * Math.Sin(startAngle + (theta * d + fractionBay));

                    }
                }
                else
                {
                    xO = centreX + radius * Math.Cos(startAngle + (theta * d));
                    yO = centreY + radius * Math.Sin(startAngle + (theta * d));
                    xP = radius * Math.Cos(startAngle + (theta * d));
                    yP = radius * Math.Sin(startAngle + (theta * d));

                }
                Point origin = Geometry.Create.Point(xO, yO, 0);

                Vector xdir = Geometry.Create.Vector(xP,yP, 0);
                
                ProfileOrigin tempPlane = Create.ProfileOrigin(origin, xdir);
                
                sectionPlane.Add(tempPlane);
            }
            return sectionPlane;
        }
        /***************************************************/
        private static Point intersectCircles(double c1x, double c1y, double c1rad, double c2x, double c2y, double c2rad)
        {
            // see http://local.wasp.uwa.edu.au/~pbourke/geometry/2circle/
            Point intersect = new Point();
            double d, a, x, y, h, prop;
            //distance between centres
            d = Math.Sqrt((c2x - c1x) * (c2x - c1x) + (c2y - c1y) * (c2y - c1y));
            if (d > c1rad + c2rad)
            {
            }// no intersection circles apart
            if (d < c1rad - c2rad)
            {
            }// no intersection cicrle inside circle
            if (c1rad == c2rad)
            {
            }// infinite intersections circles over lap

            else
            {
                a = (c1rad * c1rad - c2rad * c2rad + d * d) / (2 * d);
                h = Math.Sqrt(c1rad * c1rad - (a * a));
                prop = a / d;


                x = c1x + (prop) * (c2x - c1x);

                y = c1y + prop * (c2y - c1y);

                intersect.X = x + h * (c2y - c1y) / d;
                intersect.Y = y - h * (c2x - c1x) / d;

                intersect.X = x - h * (c2y - c1y) / d;//always gives two intersects generally we want this for the bowl layout.
                intersect.Y = y + h * (c2x - c1x) / d;
            }
            return intersect;
        }
    }
}
