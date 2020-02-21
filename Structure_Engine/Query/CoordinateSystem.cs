/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the carteseian coordinate system descibring the position and local orientation of the node in the Global coordinate system")]
        [Input("node","The Node to extract the local coordinate system from.")]
        [Output("CoordinateSystem","The local cartesian coordinate system of the Node.")]
        public static Cartesian CoordinateSystem(this Node node)
        {
            return Engine.Geometry.Create.CartesianCoordinateSystem(node.Position, node.Orientation.X, node.Orientation.Y);
        }

        /***************************************************/

        [Description("Get the carteseian coordinate system descibring the position and local orientation of the Bar in the Global coordinate system where the Bar tangent is the local x-axis and the normal is the local z-axis")]
        [Input("bar", "The Bar to extract the local coordinate system from.")]
        [Output("CoordinateSystem", "The local cartesian coordinate system of the Bar.")]
        public static Cartesian CoordinateSystem(this Bar bar)
        {
            Vector tan = bar.Tangent(true);
            Vector ax = bar.Normal().CrossProduct(tan);
            return Engine.Geometry.Create.CartesianCoordinateSystem(bar.StartNode.Position, tan, ax);
        }

        /***************************************************/
    }
}

