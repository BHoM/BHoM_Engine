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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets the geometry of a Point by replacing it with a new Point.")]
        [Input("point", "The original Point to replace.")]
        [Input("newPoint", "The new Point geometry to set.")]
        [Output("point", "The new Point geometry.")]
        public static Point SetGeometry(this Point point, Point newPoint)
        {
            return newPoint;
        }

        /***************************************************/

        [Description("Sets the geometry of a Line by replacing it with a new curve.")]
        [Input("curve", "The original Line to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this Line curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of an Arc by replacing it with a new curve.")]
        [Input("curve", "The original Arc to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this Arc curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of a Circle by replacing it with a new curve.")]
        [Input("curve", "The original Circle to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this Circle curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of an Ellipse by replacing it with a new curve.")]
        [Input("curve", "The original Ellipse to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this Ellipse curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of a NurbsCurve by replacing it with a new curve.")]
        [Input("curve", "The original NurbsCurve to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this NurbsCurve curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of a Polyline by replacing it with a new curve.")]
        [Input("curve", "The original Polyline to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this Polyline curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        [Description("Sets the geometry of a PolyCurve by replacing it with a new curve.")]
        [Input("curve", "The original PolyCurve to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve SetGeometry(this PolyCurve curve, ICurve newCurve)
        {
            return newCurve ;
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        [Description("Sets the geometry of any ICurve by replacing it with a new curve.")]
        [Input("curve", "The original ICurve to replace.")]
        [Input("newCurve", "The new curve geometry to set.")]
        [Output("curve", "The new curve geometry.")]
        public static ICurve ISetGeometry(this ICurve curve, ICurve newCurve)
        {
            return SetGeometry(curve as dynamic, newCurve);
        }

        /***************************************************/
    }
}
