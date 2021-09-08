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

        [Description("Gets the minimum scheduling radius based on the diameter of the reinforcement bar using the values given in BS 8666:2020 Table 2.")]
        [Input("diameter", "The diameter of the reinforcement bar to determine the minimum scheduling radius.")]
        [Output("radius", "The minimum scheduling radius based on the diameter of the reinforcement bar", typeof(Length))]
        public static double SchedulingRadius(this double diameter)
        {
            switch (diameter)
            {
                case 6:
                    return 12;
                case 8:
                    return 16;
                case 10:
                    return 20;
                case 12:
                    return 24;
                case 16:
                    return 32;
                case 20:
                    return 70;
                case 25:
                    return 87;
                case 32:
                    return 112;
                case 40:
                    return 140;
                case 50:
                    return 175;
                default:
                    Reflection.Compute.RecordError("The diameter of the bar provided is not a nominal size of the bar, the scheduling radius is not provided.");
                    return 0;
            }
        }

        /***************************************************/

    }
}

