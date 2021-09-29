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
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the hook diameter based on the diameter of the reinforcement bar, the shape code and the bend radius.")]
        [Input("reinforcement", "The reinforcement that contains the diameter, ShapeCode and bend radius.")]
        [Output("hookDiameter", "The anticipated hook diameter based on the diameter of the reinforcement bar", typeof(Length))]
        public static double HookDiameter(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : HookDiameter(reinforcement.ShapeCode, reinforcement.Diameter, reinforcement.BendRadius);
        }

        /***************************************************/

        [Description("Gets the hook diameter based on the diameter of the reinforcement bar, the shape code and the bend radius.")]
        [Input("shapeCode", "The ShapeCode used to determine the standard to calculate the scheduling radius.")]
        [Input("diameter", "The diameter of the reinforcement bar to determine the hook diameter.", typeof(Length))]
        [Input("bendingRadius", "The bending radius of the bar used to override the SchedulingRadius method that looks up the value in BS 8666:2020 Table 2.", typeof(Length))]
        [Output("hookDiameter", "The anticipated hook diameter based on the diameter of the reinforcement bar", typeof(Length))]
        public static double HookDiameter(this IShapeCode shapeCode, double diameter, double bendRadius = 0)
        {
            if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than 0. The scheduling radius cannot be calculated.");
                return 0;
            }

            string standard = ReinforcementStandard(shapeCode);

            switch (standard)
            {
                case "BS8666":
                    if (diameter > 0.050)
                    {
                        Reflection.Compute.RecordWarning("Bars that are greater than 50mm cannot be bent using a standard mandrel.");
                    }

                    bendRadius = bendRadius < shapeCode.SchedulingRadius(diameter) ? shapeCode.SchedulingRadius(diameter) : bendRadius;

                    return Math.Ceiling((3 * diameter + 2 * bendRadius) / 0.005) * 0.005;
                default:
                    Reflection.Compute.RecordError("Standard not recognised or supported, the scheduling radius could not be calculated.");
                    return 0;
            }
        }

        /***************************************************/

    }
}

