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

using BH.oM.Geometry;
using BH.oM.Spatial.SettingOut;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Grid in the XY Plane from a curve.")]
        [Input("curve", "Curve to be used as grid curve. Will be projected to the XY Plane.")]
        [Input("name", "Optional name of the Grid.")]
        [Output("grid", "A Grid in the XY Plane.")]
        public static Grid Grid(ICurve curve, string name = "")
        {
            return new Grid
            {
                Curve = Geometry.Modify.IProject(curve, BH.oM.Geometry.Plane.XY),
                Name = name,
            };
        }

        /***************************************************/

        [Description("Creates a linear Grid in the XY Plane from a point and a vector.")]
        [Input("origin", "Origin point of the grid line.")]
        [Input("direction", "Direction of the grid. Will be projected to the XY plane and unitized.")]
        [Input("length", "Length of the output Grid line.", typeof(Length))]
        [Input("name", "Optional name of the Grid.")]
        [Output("grid", "A Grid in the XY Plane.")]
        public static Grid Grid(Point origin, Vector direction, double length = 20, string name = "")
        {
            Point projectedOrigin = origin.Project(Plane.XY);
            Line line = new Line { Start = projectedOrigin, End = projectedOrigin + new Vector { X = direction.X, Y = direction.Y, Z = 0 }.Normalise() * length };
            return new Grid { Curve = line, Name = name };
        }

        /***************************************************/
    }
}





