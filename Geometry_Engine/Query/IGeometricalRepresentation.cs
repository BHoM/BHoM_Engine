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
        public static IGeometry GeometricalRepresentation(this Point point, ElementRepresentationOptions options = null)
        {
            options = options ?? new ElementRepresentationOptions();

            double radius = 0.15 * options.Scale;
            Sphere sphere = BH.Engine.Geometry.Create.Sphere(point, radius);

            return new GeometricalRepresentation() { Geometry = sphere, Colour = options.Colour };
        }

        /***************************************************/

        [Description("Returns the geometrical representation of the curve, which is a Pipe.")] // the pipe radius corresponds to how big the Curve is when represented.
        public static IGeometry GeometricalRepresentation(this ICurve curve, object colour = null, ElementRepresentationOptions options = null)
        {
            options = options ?? new ElementRepresentationOptions();

            double radius = 0.01 * options.Scale;
            bool capped = options.Cap1DElements;

            Pipe pipe = BH.Engine.Geometry.Create.Pipe(curve, radius, capped);

            return new GeometricalRepresentation() { Geometry = pipe, Colour = options.Colour };
        }

        /***************************************************/

        private static IGeometry GeometricalRepresentation(IObject iObj, IGeometricalRepresentationOptions options)
        {
            GeometricalRepresentation geomRepr = Reflection.Compute.RunExtensionMethod(iObj, "GeometricalRepresentation", new object[] { options }) as GeometricalRepresentation;

            if (geomRepr != null) // object has a GeometricalRepresentation method.
                return geomRepr;

            geomRepr = new GeometricalRepresentation();

            IGeometry geom = iObj as IGeometry;
            if (geom != null) // it's a geometrical object that does not have a GeometricalRepresentation method.
                geomRepr.Geometry = geom;
            else // it's a generic object that does not have a GeometricalRepresentation method.
                geom = BH.Engine.Base.Query.IGeometry(iObj as BHoMObject);

            if (options != null)
                geomRepr.Colour = options.Colour;

            return geomRepr;
        }

        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static IGeometry IGeometricalRepresentation(this IObject iObj, IGeometricalRepresentationOptions options = null)
        {
            return GeometricalRepresentation(iObj as dynamic, options as dynamic);
        }

        /***************************************************/
    }
}


