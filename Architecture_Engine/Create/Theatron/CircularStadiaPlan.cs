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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Architecture.Theatron;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static TheatronPlan CircularPlan(StadiaParameters parameters)
        {
            TheatronPlan plan = new TheatronPlan();
            circularPlaneSetUp(ref plan, parameters.TheatronRadius, parameters.StructBayWidth);
            return plan;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void circularPlaneSetUp(ref TheatronPlan plan,double radius, double structBayW)
        {
            plan.SectionOrigins = new List<ProfileOrigin>();
            
            int numBays = (int)(Math.Floor(Math.PI * radius * 2 / structBayW));
            double theta = 2 * Math.PI / numBays;
            bool halfbayStart = false;
            plan.SectionOrigins = arcSweepBay(0, 0, theta, 0, radius, numBays, halfbayStart, 1.0);
            BayType bayType = BayType.Side;
            for (int i = 0; i < plan.SectionOrigins.Count; i++)
            {
                plan.StructBayType.Add(bayType);
                
            }

        }
    }
}
