/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the hook diameter based on the diameter of the reinforcement bar using the anticipated values given in BS 8666:2020 Table 2.")]
        [Input("diameter", "The diameter of the reinforcement bar to determine the hook diameter.")]
        [Input("bendingRadius", "The bending radius of the bar used to override the SchedulingRadius method that looks up the value in BS 8666:2020 Table 2.")]
        [Output("diameter", "The anticipated hook dianeter based on the diameter of the reinforcement bar", typeof(Length))]
        public static double HookDiameter(this double diameter, double bendingRadius = 0)
        {
            if (diameter > 50)
                Reflection.Compute.RecordWarning("Bars that are greater than 50mm cannot be bent using a standard mandrel.");
            
            return Math.Ceiling((3 * diameter + 2 * bendingRadius == 0 ? diameter.SchedulingRadius() : bendingRadius) / 5) * 5;
        }

        /***************************************************/

    }
}

