/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.Physical.Reinforcement;




namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Reinforcement object storing the ShapeCode and coordinate system.")]
        [Input("coordinateSystem", "Position and orientation of the Reinforcement in Space.Refer to the description of the shape codes for how they relate to the coordinate system.")]
        [Input("shapeCode", "The name of the beam, default empty string.")]
        [Output("reinforcement", "The reinforcement object with a compliant shape code in accordance with BS 8666:2020.")]
        public static Reinforcement Reinforcement(Cartesian coordinateSystem, IShapeCode shapeCode)
        {
            Reinforcement reinforcement = new Reinforcement(){CoordinateSystem = coordinateSystem, ShapeCode = shapeCode};

            return reinforcement.IsValid() ? reinforcement : null;
        }

        /***************************************************/
    }
}
