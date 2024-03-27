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
using BH.oM.Architecture.BuildersWork;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System.ComponentModel;

namespace BH.Engine.Architecture.Elements
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Created a rectangular BuildersWork opening with given dimensions, in a given coordinate system.")]
        [InputFromProperty("coordinateSystem")]
        [Input("height", "Height of the created opening.", typeof(Length))]
        [Input("width", "Width of the created opening.", typeof(Length))]
        [InputFromProperty("depth")]
        [Output("Opening")]
        public static Opening Opening(Cartesian coordinateSystem, double height, double width, double depth)
        {
            RectangleProfile profile = BH.Engine.Spatial.Create.RectangleProfile(height, width);
            return new Opening { CoordinateSystem = coordinateSystem, Profile = profile, Depth = depth };
        }

        /***************************************************/

        [Description("Created a circular BuildersWork opening with given dimensions, in a given coordinate system.")]
        [InputFromProperty("coordinateSystem")]
        [Input("diameter", "Diameter of the created opening.", typeof(Length))]
        [InputFromProperty("depth")]
        [Output("Opening")]
        public static Opening Opening(Cartesian coordinateSystem, double diameter, double depth)
        {
            CircleProfile profile = BH.Engine.Spatial.Create.CircleProfile(diameter);
            return new Opening { CoordinateSystem = coordinateSystem, Profile = profile, Depth = depth };
        }

        /***************************************************/
    }
}




