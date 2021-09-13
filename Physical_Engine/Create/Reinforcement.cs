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
using BH.Engine.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Reinforcement;




namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Reinforcement object storing the ShapeCode, diameter, bend radius and coordinate system.")]
        [Input("diameter", "The diameter of the reinforcement.")]
        [Input("coordinateSystem", "The name of the beam, default empty string")]
        [Input("shapeCode", "The name of the beam, default empty string")]
        [Input("bendRadius", "The bend radius of the reinforcement. This will be calculated based on the diameter if the provided value " +
            "is less than the the minimum scheduling radius defined in BS 8666:2020.")]
        [Output("reinforcement", "The reinforcement object with a compliant shape code in accordance with BS 8666:2020.")]
        public static Reinforcement Reinforcement(double diameter, Cartesian coordinateSystem, IShapeCode shapeCode, double bendRadius = 0)
        {
            if (shapeCode.IsNull())
                return null;
            else if (coordinateSystem.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }
            else if (coordinateSystem.IsNull())
                return null;
            else if (bendRadius < diameter.SchedulingRadius())
            {
                bendRadius = diameter.SchedulingRadius();
                Reflection.Compute.RecordWarning("The bend radius is less than the minimum scheduling radius and has been assigned the " +
                    "minimum value.");
            }

            return new Reinforcement()
            {
                Diameter = diameter,
                BendRadius = bendRadius,
                CoordinateSystem = coordinateSystem,
                ShapeCode = shapeCode
            };
        }

        /***************************************************/
    }
}