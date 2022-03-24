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
using BH.oM.Base.Attributes;

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
            return reinforcement.IsNull() ? 0 : HookDiameter(reinforcement.ShapeCode);
        }

        /***************************************************/

        [Description("Gets the hook diameter based on the diameter of the reinforcement bar, the shape code and the bend radius.")]
        [Input("shapeCode", "The ShapeCode used to determine the standard to calculate the scheduling radius.")]
        [Output("hookDiameter", "The anticipated hook diameter based on the diameter of the reinforcement bar", typeof(Length))]
        public static double HookDiameter(this IShapeCode shapeCode)
        {
            if (shapeCode.IsNull())
                return 0;

            string standard = ReinforcementStandard(shapeCode);

            switch (standard)
            {
                case "BS8666":
                    if (shapeCode.Diameter > 0.050)
                    {
                        Base.Compute.RecordWarning("Bars that are greater than 50mm cannot be bent using a standard mandrel.");
                    }

                    return Math.Ceiling((3 * shapeCode.Diameter + 2 * shapeCode.BendRadius) / 0.005) * 0.005;
                default:
                    Base.Compute.RecordError("Standard not recognised or supported, the scheduling radius could not be calculated.");
                    return 0;
            }
        }

        /***************************************************/

    }
}

