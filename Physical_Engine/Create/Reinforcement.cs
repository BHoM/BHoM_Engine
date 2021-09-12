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

        [Description("Creates a physical Beam element. To generate elements compatible with structural packages, have a look at the Bar class")]
        [Input("diameter", "The centre line geometry of the beam")]
        [Input("bendRadius", "The property of the beam, containing its profile, orientation and materiality")]
        [Input("coordinateSystem", "The name of the beam, default empty string")]
        [Input("shapeCode", "The name of the beam, default empty string")]
        [Output("Beam", "The created physical beam")]
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


