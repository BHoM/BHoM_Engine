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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the geometrical identity of any IBHoMObject, useful for diffing. " +
            "\nThe geometrical identity is computed by extracting the geometry of the object via the IGeometry() method." +
            "\nThen, the hash is computed as a serialised array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Input("bhomObj", "Input BHoMObject whose geometry will be queried by IGeometry() and which will be used for computing a Geometry Hash.")]
        [Output("geomHash", "Number representing a unique signature of the input object's geometry.")]
        public static string GeometryHash(this IBHoMObject bhomObj)
        {
            IGeometry igeom = bhomObj.IGeometry();

            return GeometryHash(igeom);
        }

        /***************************************************/

        [Description("Returns a signature of the input geometry, useful for diffing." +
            "\nThe hash is computed as a serialised array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Output("geomHash", "Number representing a unique signature of the input geometry.")]
        public static string GeometryHash(this IGeometry igeometry)
        {
            if (igeometry == null)
                return null;

            double[] hashArray = IHashArray(igeometry);

            double infinity = 1e12; // value compatible with the aggregation.

            long result = 0;
            int j = 1;
            for (int i = 0; i < hashArray.Length; i++)
            {
                double num = hashArray[i];

                if (double.IsNaN(num))
                    throw new ArgumentException("Cannot calculate the GeometryHash if the input geometries have a non-determined (NaN) value.");
                else if (double.IsPositiveInfinity(num))
                    num = infinity;
                else if (double.IsNegativeInfinity(num))
                    num = -infinity;

                result += (long)((num * (1 / Tolerance.Distance)) * (j * m_aggregationMultiplier) * (1 / m_aggregationMultiplier));

                if (m_aggregationMultiplierWrapAfter < 0)
                    j++;
                else
                    j = (j < m_aggregationMultiplierWrapAfter) ? (j + 1) : 1;
            }

            return result.ToString();
        }

        /***************************************************/
        /****  Private fields                           ****/
        /***************************************************/

        [Description("The hash aggregation (array summing) requires that individual numbers get multiplied for different multipliers before being summed," +
            "in order to avoid incurring in the same aggregated hash for e.g. two points like (1,0,0) and (0,1,0).\n" +
            "This is the multiplier used when summing the Hash Arrays to compose a single numerical hash;" +
            "it is increased by a factor of 1 for subsequent hash array numbers.")]
        private const double m_aggregationMultiplier = 1e-18; // this brings it just over the `long` digits limit

        [Description("During the aggregation process, m_aggregationMultiplier (see its description) increases by a factor of 1 for subsequent hash array numbers, " +
            "until m_aggregationMultiplierWrapAfter is reached, after which the factor re-starts from 0." +
            "Defaults to -1; values < 0 disable wrapping, i.e. all aggregated numbers get multiplied by a different multiplier.")]
        private const int m_aggregationMultiplierWrapAfter = -1;
    }
}