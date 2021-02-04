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
        /****              Interface Methods            ****/
        /***************************************************/

        public static GeometricalRepresentation IGeometricalRepresentation(this IObject iObj, IGeometricalRepresentationOptions options = null)
        {
            return GeometricalRepresentation(iObj as dynamic, options as dynamic);
        }

        /***************************************************/
        /****         Private methods - fallback        ****/
        /***************************************************/

        private static GeometricalRepresentation GeometricalRepresentation(IObject iObj, IGeometricalRepresentationOptions options = null)
        {
            GeometricalRepresentation geomRepr = iObj as GeometricalRepresentation;
            if (geomRepr != null)
                return geomRepr;

            geomRepr = Reflection.Compute.RunExtensionMethod(iObj, "Representation", new object[] { options }) as GeometricalRepresentation;

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
    }
}


