/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the minimum scheduling radius based on the diameter of the reinforcement bar. The standard is determined from the namespace of the ShapeCode.")]
        [Input("reinforcement", "The reinforcement that contains the diameter and the ShapeCode.")]
        [Output("schedulingRadius", "The minimum scheduling radius based on the diameter of the reinforcement bar to the standard of the ShapeCode.", typeof(Length))]
        public static double SchedulingRadius(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : SchedulingRadius(reinforcement.ShapeCode);
        }

        /***************************************************/

        [Description("Gets the minimum scheduling radius based on the diameter of the reinforcement bar. The standard is determined from the namespace of the ShapeCode.")]
        [Input("shapeCode", "The ShapeCode used to determine the standard to calculate the scheduling radius.")]
        [Output("schedulingRadius", "The minimum scheduling radius based on the diameter of the reinforcement bar to the standard of the ShapeCode.", typeof(Length))]
        public static double SchedulingRadius(this IShapeCode shapeCode)
        {
            if (shapeCode.IsNull())
                return 0;

            string standard = ReinforcementStandard(shapeCode);

            return SchedulingRadius(shapeCode.Diameter, standard);
        }

        /***************************************************/

        [Description("Gets the minimum scheduling radius based on the diameter of the reinforcement bar. The standard is determined from the namespace of the ShapeCode.")]
        [Input("diameter", "The diameter used to calculate the scheduling radius.")]
        [Output("schedulingRadius", "The minimum scheduling radius based on the diameter of the reinforcement bar to the standard of the ShapeCode.", typeof(Length))]
        public static double SchedulingRadius<T>(this double diameter) where T : IShapeCode
        {
            string standard = ReinforcementStandard(typeof(T));

            return SchedulingRadius(diameter, standard);
        }

        /***************************************************/
        private static double SchedulingRadius(this double diameter, string standard)
        {
            switch (standard)
            {
                case "BS8666":
                    if (diameter < 0.020)
                        return 2 * diameter;
                    else
                        return 3.5 * diameter;
                default:
                    Base.Compute.RecordError("Standard not recognised or supported, the scheduling radius could not be calculated.");
                    return 0;
            }
        }

        /***************************************************/

    }
}



