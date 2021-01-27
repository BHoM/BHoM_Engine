/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Geometry.SettingOut;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the geometrical representation of the point, which is a Sphere.")] // in the future, we might want an option to choose between sphere / box.
        public static IGeometry GeometricalRepresentation(this Point point, object colour = null, GeometricalRepresentationOptions options = null)
        {
            options = options ?? new GeometricalRepresentationOptions();

            double radius = 0.15 * options.Scale;
            Sphere sphere = BH.Engine.Geometry.Create.Sphere(point, radius);

            return new GeometricalRepresentation() { Geometry = sphere, ColourInfo = colour };
        }

        /***************************************************/

        [Description("Returns the geometrical representation of the curve, which is a Pipe.")] // the pipe radius corresponds to how big the Curve is when represented.
        public static IGeometry GeometricalRepresentation(this ICurve curve, object colour = null, GeometricalRepresentationOptions options = null)
        {
            options = options ?? new GeometricalRepresentationOptions();

            double radius = 0.01 * options.Scale;
            bool capped = options.Cap1DElements;

            return BH.Engine.Geometry.Create.Pipe(curve, radius, capped);
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static IGeometry IGeometricalRepresentation(this IObject iObj, IRepresentationOptions options = null)
        {
            return GeometricalRepresentation(iObj as dynamic, options as dynamic);
        }

        /***************************************************/
    }
}


