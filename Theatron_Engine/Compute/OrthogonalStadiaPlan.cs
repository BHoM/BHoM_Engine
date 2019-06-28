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

using BH.oM.Geometry;
using BH.oM.Theatron.Elements;
using BH.oM.Theatron.Parameters;
using System;
using System.Collections.Generic;

namespace BH.Engine.Theatron
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static TheatronPlanGeometry OrthogonalPlan(StadiaParameters parameters)
        {
            TheatronPlanGeometry plan = new TheatronPlanGeometry();
            orthoPlanSetUp(ref plan, parameters.PitchWidth, parameters.PitchLength, parameters.EndBound, parameters.SideBound, 
                parameters.CornerRadius, parameters.StructBayWidth, parameters.NumCornerBays);
            return plan;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void orthoPlanSetUp(ref TheatronPlanGeometry plan, double playWidth, double playLength, double endBound, double sideBound, double cornerR, double structBayW, int nCornerBays)
        {
            int nSideBays = (int)(Math.Floor(((playLength + endBound) / 2 - cornerR) / structBayW) * 2);
            int nEndBays = (int)(Math.Floor(((playWidth + sideBound) / 2 - cornerR) / structBayW) * 2);
            plan.SectionPlanes = new List<Plane>();
            
            double cornA = Math.PI / 2 / (nCornerBays + 1);
            double trueR = cornerR / Math.Cos(cornA / 2);
            double xMin;
            double yMin;
            double oX, oY, dX, dY;
            int count = 0;
            Point origin = new Point();
            Point xdir = new Point();
            Point ydir = new Point();
            Plane tempPlane = new Plane();
            BayType bayType = 0; //0 = side, 1= end, 2 =corner
            for (int i = 0; i < 8; i++)
            {
                if (i == 0 || i == 4)//side bay
                {
                    bayType = 0;
                    for (int d = 0; d <= nSideBays; d++)
                    {
                        plan.StructBayType.Add(bayType);
                        if (i == 0)//right side
                        {
                            yMin = (nSideBays * structBayW) / -2;
                            //origin xyz 
                            oX = ((playWidth + sideBound) / 2);
                            oY = yMin + (structBayW * d);
                            dX = 1;
                            dY = 0;
                        }
                        else//left side
                        {
                            yMin = (nSideBays * structBayW) / 2;
                            //origin xyz 
                            oX = -((playWidth + sideBound) / 2);
                            oY = yMin - (structBayW * d);
                            dX = -1;
                            dY = 0;
                        }
                        origin = Geometry.Create.Point(oX, oY, 0);
                        
                        xdir = Geometry.Create.Point(oX + dX,oY + dY, 0);
                        ydir = Geometry.Create.Point(oX, oY, 1);
                        tempPlane = Geometry.Create.Plane(origin, xdir, ydir);
                        plan.SectionPlanes.Add(tempPlane);
                        
                        count++;
                    }
                }
                else
                {

                    if (i == 2 || i == 6)//end bay
                    {
                        bayType = BayType.End;
                        xMin = (nEndBays * structBayW) / 2;
                        //                                    
                        for (int d = 0; d <= nEndBays; d++)
                        {
                            plan.StructBayType.Add(bayType);
                            if (i == 2)// northEnd
                            {
                                //origin xyz 
                                oX = xMin - (structBayW * d);
                                oY = ((playLength + endBound) / 2);
                                dX = 0;
                                dY = 1;
                            }
                            else
                            {
                                //origin xyz 
                                oX = -xMin + (structBayW * d);
                                oY = -((playLength + endBound) / 2);

                                dX = 0;
                                dY = -1;
                            }
                            origin = Geometry.Create.Point(oX, oY, 0);
                            
                            xdir = Geometry.Create.Point(oX + dX,oY + dY, 0);
                            ydir = Geometry.Create.Point(oX, oY, 1);
                            tempPlane = Geometry.Create.Plane(origin, xdir, ydir);
                            plan.SectionPlanes.Add(tempPlane);
                            
                            count++;
                        }
                    }
                    else//corners
                    {
                        //local centres cs at fillets
                        bayType = BayType.Corner;
                        double centreX = (playWidth + sideBound) / 2 - cornerR;
                        double centreY = (playLength + endBound) / 2 - cornerR;
                        double startAngle = 0;
                        if (i == 1) //NE++
                        {

                        }
                        if (i == 3) //NW-+
                        {
                            centreX = -centreX;
                            startAngle = Math.PI / 2;
                        }
                        if (i == 5) //SW--
                        {
                            centreY = -centreY;
                            centreX = -centreX;
                            startAngle = Math.PI;
                        }
                        if (i == 7) //SE+-
                        {
                            centreY = -1 * centreY;
                            startAngle = Math.PI * 1.5;
                        }

                        //                                    
                        for (int d = 0; d <= nCornerBays; d++)
                        {
                            plan.StructBayType.Add(bayType);
                            if (d == 0)//half bay on first
                            {
                                oX = centreX + trueR * Math.Cos(startAngle + cornA / 2);
                                oY = centreY + trueR * Math.Sin(startAngle + cornA / 2);
                                dX = trueR * Math.Cos(startAngle + cornA / 2);
                                dY = trueR * Math.Sin(startAngle + cornA / 2);
                            }
                            else
                            {
                                oX = centreX + trueR * Math.Cos(startAngle + (cornA * d + cornA / 2));
                                oY = centreY + trueR * Math.Sin(startAngle + (cornA * d + cornA / 2));
                                dX = trueR * Math.Cos(startAngle + (cornA * d + cornA / 2));
                                dY = trueR * Math.Sin(startAngle + (cornA * d + cornA / 2));
                            }
                            origin = Geometry.Create.Point(oX, oY, 0);
                            
                            xdir = Geometry.Create.Point(oX + dX,oY + dY, 0);
                            ydir = Geometry.Create.Point(oX, oY, 1);
                            tempPlane = Geometry.Create.Plane(origin, xdir, ydir);
                            plan.SectionPlanes.Add(tempPlane);
                            
                            count++;
                        }
                    }
                }
            }

        }
    }
}
