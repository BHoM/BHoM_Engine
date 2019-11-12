/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.SettingOut;

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

        public static ICurve Geometry(this Grid grid)
        {
            return grid.Curve;
        }

        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static Point IGeometry(this IElement0D element0D)
        {
            return Reflection.Compute.RunExtensionMethod(element0D, "Geometry") as Point;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static ICurve IGeometry(this IElement1D element1D)
        {
            return Reflection.Compute.RunExtensionMethod(element1D, "Geometry") as ICurve;
        }

        /******************************************/


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
