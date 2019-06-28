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
        public static TheatronPlanGeometry NoCornersPlan (StadiaParameters parameters)
        {
            TheatronPlanGeometry plan = new TheatronPlanGeometry();
            noCornerPlanSetUp(ref plan, parameters.PitchWidth, parameters.PitchLength, parameters.EndBound, parameters.StructBayWidth, parameters.SideBound);
            return plan;
        }
        
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void noCornerPlanSetUp(ref TheatronPlanGeometry plan,double playWidth, double playLength, double endBound, double structBayW, double sideBound)
        {
            int nSideBays = (int)(Math.Floor(((playLength + endBound) / 2) / structBayW) * 2);
            int nEndBays = (int)(Math.Floor(((playWidth + sideBound) / 2) / structBayW) * 2);
            plan.SectionPlanes = new List<Plane>();
            
            double actualBayW;
            double xMin;
            double yMin;
            double oX, oY, dX, dY;
            int count = 0;
            Point origin = new Point();
            Point xdir = new Point();
            Point ydir = new Point();
            Plane tempPlane = new Plane();
            BayType bayType = BayType.Side;//0 = side, 1= end, 2 =corner
            for (int i = 0; i < 4; i++)
            {

                if (i % 2 == 0)//side bay
                {
                    bayType = 0;
                    actualBayW = (playLength + endBound) / nSideBays;
                    for (int d = 0; d <= nSideBays; d++)
                    {
                        plan.StructBayType.Add(bayType);
                        if (i == 0)//right side
                        {
                            yMin = (nSideBays * actualBayW) / -2;
                            //origin xyz 
                            oX = ((playWidth + sideBound) / 2);
                            oY = yMin + (actualBayW * d);
                            dX = 1;
                            dY = 0;
                        }
                        else//left side
                        {
                            yMin = (nSideBays * actualBayW) / 2;
                            //origin xyz 
                            oX = -((playWidth + sideBound) / 2);
                            oY = yMin - (actualBayW * d);
                            dX = -1;
                            dY = 0;
                        }
                        origin =Geometry.Create.Point(oX, oY, 0);
                        
                        xdir = Geometry.Create.Point(oX + dX, oY + dY, 0);
                        ydir = Geometry.Create.Point(oX, oY , 1);
                        tempPlane = Geometry.Create.Plane(origin, xdir, ydir);
                        plan.SectionPlanes.Add(tempPlane);
                        
                        count++;
                    }
                }

                else
                {
                    bayType = BayType.End;
                    actualBayW = (playWidth + sideBound) / nEndBays;
                    xMin = (nEndBays * actualBayW) / 2;
                    //                                    
                    for (int d = 0; d <= nEndBays; d++)
                    {
                        plan.StructBayType.Add(bayType);
                        if (i == 1)// northEnd
                        {
                            //origin xyz 
                            oX = xMin - (actualBayW * d);
                            oY = ((playLength + endBound) / 2);

                            dX = 0;
                            dY = 1;// 0;
                        }
                        else
                        {
                            //origin xyz 
                            oX = -xMin + (actualBayW * d);
                            oY = -((playLength + endBound) / 2);
                            dX = 0;
                            dY = -1;
                        }
                        origin = Geometry.Create.Point(oX, oY, 0);
                        
                        xdir = Geometry.Create.Point(oX+dX,oY+ dY, 0);
                        ydir = ydir = Geometry.Create.Point(oX, oY, 1);
                        tempPlane = Geometry.Create.Plane(origin, xdir, ydir);
                        plan.SectionPlanes.Add(tempPlane);
                        
                        count++;
                    }

                }

            }

        }
    }
}
