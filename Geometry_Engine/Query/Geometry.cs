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
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the geometry of a Point, which is the Point itself.")]
        [Input("point", "The Point to get the geometry of.")]
        [Output("point", "The Point geometry.")]
        public static Point Geometry(this Point point)
        {
            return point;
        }

        /***************************************************/

        [Description("Returns the geometry of a Line as an ICurve.")]
        [Input("curve", "The Line to get the geometry of.")]
        [Output("curve", "The Line as an ICurve.")]
        public static ICurve Geometry(this Line curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of an Arc as an ICurve.")]
        [Input("curve", "The Arc to get the geometry of.")]
        [Output("curve", "The Arc as an ICurve.")]
        public static ICurve Geometry(this Arc curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of a Circle as an ICurve.")]
        [Input("curve", "The Circle to get the geometry of.")]
        [Output("curve", "The Circle as an ICurve.")]
        public static ICurve Geometry(this Circle curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of an Ellipse as an ICurve.")]
        [Input("curve", "The Ellipse to get the geometry of.")]
        [Output("curve", "The Ellipse as an ICurve.")]
        public static ICurve Geometry(this Ellipse curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of a NurbsCurve as an ICurve.")]
        [Input("curve", "The NurbsCurve to get the geometry of.")]
        [Output("curve", "The NurbsCurve as an ICurve.")]
        public static ICurve Geometry(this NurbsCurve curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of a Polyline as an ICurve.")]
        [Input("curve", "The Polyline to get the geometry of.")]
        [Output("curve", "The Polyline as an ICurve.")]
        public static ICurve Geometry(this Polyline curve)
        {
            return curve;
        }

        /***************************************************/

        [Description("Returns the geometry of a PolyCurve as an ICurve.")]
        [Input("curve", "The PolyCurve to get the geometry of.")]
        [Output("curve", "The PolyCurve as an ICurve.")]
        public static ICurve Geometry(this PolyCurve curve)
        {
            return curve;
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        [Description("Returns the geometry of any ICurve as an ICurve.")]
        [Input("curve", "The ICurve to get the geometry of.")]
        [Output("curve", "The ICurve geometry.")]
        public static ICurve IGeometry(this ICurve curve)
        {
            return Geometry(curve as dynamic);
        }

        /***************************************************/
    }
}
