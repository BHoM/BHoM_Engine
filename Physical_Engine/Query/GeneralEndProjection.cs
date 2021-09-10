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
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the minimum end projection for general bends (bobs) or links where the bend is greater than or equal to 150 degrees." +
            "This is based on the diameter of the reinforcement bar using the values given in BS 8666:2020 Table 2.")]
        [Input("diameter", "The diameter of the reinforcement bar to determine the minimum scheduling radius.")]
        [Output("endProjection", "The minimum end projection based on the diameter of the reinforcement bar", typeof(Length))]
        public static double GeneralEndProjection(this double diameter)
        {
            if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than 0. The scheduling radius cannot be provided.");
                return 0;
            }

            return Math.Max(5 * diameter, 0.090) + diameter + diameter.SchedulingRadius();
        }

        /***************************************************/

    }
}

