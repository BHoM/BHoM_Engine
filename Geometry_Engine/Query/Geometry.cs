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

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Point Geometry(this Point point)
        {
            return point;
        }

        /***************************************************/

        public static ICurve Geometry(this Line curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Arc curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Circle curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Ellipse curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this NurbsCurve curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Polyline curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this PolyCurve curve)
        {
            return curve;
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static ICurve IGeometry(this ICurve curve)
        {
            return Geometry(curve as dynamic);
        }

        /***************************************************/
    }
}






