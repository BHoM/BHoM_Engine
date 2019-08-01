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
        public static TheatronPlan OrthogonalPlan(StadiaParameters parameters)
        {
            TheatronPlan plan = new TheatronPlan();
            orthoPlanSetUp(ref plan, parameters.PitchLength,parameters.PitchWidth, parameters.EndBound, parameters.SideBound, 
                parameters.CornerRadius, parameters.StructBayWidth, parameters.NumCornerBays);
            return plan;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void orthoPlanSetUp(ref TheatronPlan plan, double length,double width, double endBound, double sideBound, double cornerR, double structBayW, int nCornerBays)
        {
            int nSideBays = (int)(Math.Floor(((length + endBound) / 2 - cornerR) / structBayW) * 2);
            int nEndBays = (int)(Math.Floor(((width + sideBound) / 2 - cornerR) / structBayW) * 2);
            plan.SectionOrigins = new List<ProfileOrigin>();
            
            double cornA = Math.PI / 2 / (nCornerBays + 1);
            double trueR = cornerR / Math.Cos(cornA / 2);
            double xMin;
            double yMin;
            double oX, oY, dX, dY;
            int count = 0;
            Point origin = new Point();
            Vector xdir = new Vector();
            Vector ydir =  Vector.ZAxis;
            Vector normal = new Vector();
            ProfileOrigin tempOrigin = new ProfileOrigin();
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
                            oX = ((width + sideBound) / 2);
                            oY = yMin + (structBayW * d);
                            dX = 1;
                            dY = 0;
                        }
                        else//left side
                        {
                            yMin = (nSideBays * structBayW) / 2;
                            //origin xyz 
                            oX = -((width + sideBound) / 2);
                            oY = yMin - (structBayW * d);
                            dX = -1;
                            dY = 0;
                        }
                        origin = Geometry.Create.Point(oX, oY, 0);
                        xdir = Geometry.Create.Vector(dX,dY, 0);
                        tempOrigin = Create.ProfileOrigin(origin, xdir);
                        plan.SectionOrigins.Add(tempOrigin);
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
                                oY = ((length + endBound) / 2);
                                dX = 0;
                                dY = 1;
                            }
                            else
                            {
                                //origin xyz 
                                oX = -xMin + (structBayW * d);
                                oY = -((length + endBound) / 2);

                                dX = 0;
                                dY = -1;
                            }
                            origin = Geometry.Create.Point(oX, oY, 0);
                            xdir = Geometry.Create.Vector(dX, dY, 0);
                            tempOrigin = Create.ProfileOrigin(origin, xdir);
                            plan.SectionOrigins.Add(tempOrigin);
                            count++;
                        }
                    }
                    else//corners
                    {
                        //local centres cs at fillets
                        bayType = BayType.Corner;
                        double centreX = (width + sideBound) / 2 - cornerR;
                        double centreY = (length + endBound) / 2 - cornerR;
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
                            xdir = Geometry.Create.Vector(dX, dY, 0);
                            tempOrigin = Create.ProfileOrigin(origin, xdir);
                            plan.SectionOrigins.Add(tempOrigin);
                            count++;
                        }
                    }
                }
            }

        }
    }
}
