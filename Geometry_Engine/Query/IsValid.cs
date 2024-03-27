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

using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsValid(this Point point)
        {
            return !(double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z));
        }

        /***************************************************/

        public static bool IsValid(this Vector v)
        {
            return !(double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z));
        }


        /***************************************************/
        /**** Public Methods - Abstract                 ****/
        /***************************************************/

        public static bool IsValid(this TransformMatrix transform)
        {
            return transform?.Matrix != null && transform.Matrix.GetLength(0) == 4 && transform.Matrix.GetLength(1) == 4;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsValid(this IGeometry geometry)
        {
            return true;
        }

        /***************************************************/

        public static bool IsValid(this Arc arc, double tolerance = Tolerance.Distance)
        {
            //TODO: Returning true for all for now until method is expanded to all objects
            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static bool IIsValid(this IGeometry geometry)
        {
            return IsValid(geometry as dynamic);
        }

        /***************************************************/
    }
}





