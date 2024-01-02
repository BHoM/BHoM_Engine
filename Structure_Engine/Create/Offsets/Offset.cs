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

using BH.oM.Structure.Offsets;
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

        [Description("Creates an Offset defining offsets for Bar objects from its end Nodes to be applied in analysis packages.")]
        [Input("startX", "Offset of the StartNode along the local x-axis, i.e. along the tangent.", typeof(Length))]
        [Input("startY", "Offset of the StartNode along the local y-axis, i.e. along the axis perpendicular to the normal and tangent.", typeof(Length))]
        [Input("startZ", "Offset of the StartNode along the local z-axis, i.e. along the normal.", typeof(Length))]
        [Input("endX", "Offset of the EndNode along the local x-axis, i.e. along the tangent.", typeof(Length))]
        [Input("endY", "Offset of the EndNode along the local y-axis, i.e. along the axis perpendicular to the normal and tangent.", typeof(Length))]
        [Input("endZ", "Offset of the EndNode along the local z-axis, i.e. along the normal.", typeof(Length))]
        [Output("offset", "The created Offset.")]
        public static Offset Offset(double startX, double startY, double startZ, double endX, double endY, double endZ)
        {
            return new Offset
            {
                Start = new oM.Geometry.Vector() { X = startX, Y = startY, Z = startZ },
                End = new oM.Geometry.Vector() { X = endX, Y = endY, Z = endZ }
            };
        }

        /***************************************************/
    }
}





