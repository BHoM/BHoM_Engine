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


using BH.oM.Architecture.Theatron;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Architecture.Theatron
{
    public class TheatronTest
    {
        public void Test()
        {
            var sType = Create.StadiaTypeEnum(10);
            var sParams = Create.StadiaParameters(7.5,10,typeOfBowl:sType);
            sParams.TypeOfBowl = StadiaType.Orthogonal;
            var plan = Create.PlanGeometry(sParams);
            var pParams1 = Create.ProfileParameters(1);
            var pParams2 = Create.ProfileParameters(1);
            pParams2.NumRows = 20;
            List<ProfileParameters> parameters = new List<ProfileParameters> { pParams1, pParams2 };
            //SIMPLE profile in XZ plane focal point is the origin
            var fullProfile = Create.TheatronFullProfile(parameters);
            //profile in XZ plane focal point is the origin but distance from origin defined by the plan geometry
            fullProfile = Create.TheatronFullProfile(parameters,plan);
        }
        public void PlaneByPointsTest()
        {
            double theta = Math.PI * 2 / 100;
            List<Plane> planes = new List<Plane>();
            Plane pln = new Plane();
            Point xpt = new Point();
            Point origin = new Point();
            Point ypt = Geometry.Create.Point(0, 0, 1);
            double rad = 100;
            double x, y;
            for(int i = 0; i< 100; i++)
            {
                x = rad * Math.Cos(theta * i);
                y = rad * Math.Sin(theta * i);
                origin = Geometry.Create.Point(x, y, 0);
                x = (rad+10) * Math.Cos(theta * i);
                y = (rad + 10) * Math.Sin(theta * i);
                xpt = Geometry.Create.Point(x, y, 0);
                pln = Geometry.Create.Plane(origin, xpt, ypt);

            }
        }
    }
}
