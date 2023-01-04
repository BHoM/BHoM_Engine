/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Point SetGeometry(this Point point, Point newPoint)
        {
            return newPoint.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Line curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Arc curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Circle curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Ellipse curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this NurbsCurve curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Polyline curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this PolyCurve curve, ICurve newCurve)
        {
            return newCurve.DeepClone();
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static ICurve ISetGeometry(this ICurve curve, ICurve newCurve)
        {
            return SetGeometry(curve as dynamic, newCurve);
        }

        /***************************************************/
    }
}




