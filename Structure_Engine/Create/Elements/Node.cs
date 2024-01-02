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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Node from a Cartesian coordinate system. The position of the Node will be the Orgin, and the Orientation of the node will match the axes of the Coordinate system.")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the Node.")]
        [Input("name", "The name of the created Node.")]
        [InputFromProperty("support")]
        [Output("node", "The created structural Node.")]
        public static Node Node(Cartesian coordinates, string name = "", Constraint6DOF support = null)
        {
            return coordinates.IsNull() ? null : new Node
            {
                Position = coordinates.Origin,
                Orientation = (Basis)coordinates,
                Name = name,
                Support = support
            };
        }

        /***************************************************/

    }
}





