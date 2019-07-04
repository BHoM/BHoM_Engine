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
using System.Collections.Generic;

namespace BH.Engine.Architecture.Theatron
{
    public class TheatronTest
    {
        public void Test()
        {
            var sParams = Create.StadiaParameters(1);
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
    }
}
