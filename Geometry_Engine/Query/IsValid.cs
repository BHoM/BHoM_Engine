/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Checks if a Point is valid by ensuring none of its coordinates are NaN.")]
        [Input("point", "The Point to validate.")]
        [Output("isValid", "True if the Point is valid, false otherwise.")]
        public static bool IsValid(this Point point)
        {
            return !(double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z));
        }

        /***************************************************/

        [Description("Checks if a Vector is valid by ensuring none of its components are NaN.")]
        [Input("v", "The Vector to validate.")]
        [Output("isValid", "True if the Vector is valid, false otherwise.")]
        public static bool IsValid(this Vector v)
        {
            return !(double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z));
        }


        /***************************************************/
        /**** Public Methods - Abstract                 ****/
        /***************************************************/

        [Description("Checks if a TransformMatrix is valid by ensuring it has the correct dimensions (4x4).")]
        [Input("transform", "The TransformMatrix to validate.")]
        [Output("isValid", "True if the TransformMatrix is valid, false otherwise.")]
        public static bool IsValid(this TransformMatrix transform)
        {
            return transform?.Matrix != null && transform.Matrix.GetLength(0) == 4 && transform.Matrix.GetLength(1) == 4;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if any IGeometry is valid. Currently returns true for all geometries.")]
        [Input("geometry", "The IGeometry to validate.")]
        [Output("isValid", "True if the geometry is valid, false otherwise.")]
        public static bool IsValid(this IGeometry geometry)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if an Arc is valid. Currently returns true for all arcs.")]
        [Input("arc", "The Arc to validate.")]
        [Input("tolerance", "The tolerance for the validation check.", typeof(Length))]
        [Output("isValid", "True if the Arc is valid, false otherwise.")]
        public static bool IsValid(this Arc arc, double tolerance = Tolerance.Distance)
        {
            //TODO: Returning true for all for now until method is expanded to all objects
            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Checks if any IGeometry is valid using dynamic dispatch.")]
        [Input("geometry", "The IGeometry to validate.")]
        [Output("isValid", "True if the geometry is valid, false otherwise.")]
        public static bool IIsValid(this IGeometry geometry)
        {
            return IsValid(geometry as dynamic);
        }

        /***************************************************/
    }
}
