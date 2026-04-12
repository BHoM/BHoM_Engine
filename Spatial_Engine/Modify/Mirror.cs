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

using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Mirrors the geometrical definition and location-dependent properties of an IElement against the given plane.")]
        [Input("element", "IElement to mirror.")]
        [Input("plane", "Mirror plane.")]
        [Output("mirrored", "Modified IElement with unchanged general properties, but mirrored geometrical definition and location-dependent properties.")]
        public static IElement IMirror(this IElement element, Plane plane)
        {
            return IMirror(element as dynamic, plane);
        }

        /***************************************************/

        [Description("Mirrors the geometrical definition and location-dependent properties of an IElement2D against the given plane.")]
        [Input("element2D", "IElement2D to mirror.")]
        [Input("plane", "Mirror plane.")]
        [Output("mirrored", "Modified IElement2D with unchanged general properties, but mirrored geometrical definition and location-dependent properties.")]
        public static IElement2D IMirror(this IElement2D element2D, Plane plane)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element2D, nameof(Mirror), new object[] { plane }, out result))
                result = element2D.Mirror(plane);

            return result as IElement2D;
        }

        /***************************************************/

        [Description("Mirrors the geometrical definition and location-dependent properties of an IElement1D against the given plane.")]
        [Input("element1D", "IElement1D to mirror.")]
        [Input("plane", "Mirror plane.")]
        [Output("mirrored", "Modified IElement1D with unchanged general properties, but mirrored geometrical definition and location-dependent properties.")]
        public static IElement1D IMirror(this IElement1D element1D, Plane plane)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element1D, nameof(Mirror), new object[] { plane }, out result))
                result = element1D.Mirror(plane);

            return result as IElement1D;
        }

        /***************************************************/

        [Description("Mirrors the geometrical definition and location-dependent properties of an IElement0D against the given plane.")]
        [Input("element0D", "IElement0D to mirror.")]
        [Input("plane", "Mirror plane.")]
        [Output("mirrored", "Modified IElement0D with unchanged general properties, but mirrored geometrical definition and location-dependent properties.")]
        public static IElement0D IMirror(this IElement0D element0D, Plane plane)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element0D, nameof(Mirror), new object[] { plane }, out result))
                result = element0D.Mirror(plane);

            return result as IElement0D;
        }


        /***************************************************/
        /**** Private Methods - IElements               ****/
        /***************************************************/

        private static IElement2D Mirror(this IElement2D element2D, Plane plane)
        {
            List<IElement1D> newOutline = new List<IElement1D>();
            foreach (IElement1D element1D in element2D.IOutlineElements1D())
            {
                newOutline.Add(element1D.Mirror(plane));
            }
            IElement2D result = element2D.ISetOutlineElements1D(newOutline);

            List<IElement2D> newInternalOutlines = new List<IElement2D>();
            foreach (IElement2D internalElement2D in result.IInternalElements2D())
            {
                newInternalOutlines.Add(internalElement2D.Mirror(plane));
            }
            result = result.ISetInternalElements2D(newInternalOutlines);
            return result;
        }

        /***************************************************/

        private static IElement1D Mirror(this IElement1D element1D, Plane plane)
        {
            return element1D.ISetGeometry(Geometry.Modify.IMirror(element1D.IGeometry(), plane));
        }

        /***************************************************/

        private static IElement0D Mirror(this IElement0D element0D, Plane plane)
        {
            return element0D.ISetGeometry(Geometry.Modify.Mirror(element0D.IGeometry(), plane));
        }

        /***************************************************/
    }
}







