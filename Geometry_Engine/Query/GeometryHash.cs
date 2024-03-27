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
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [PreviousVersion("7.1", "BH.Engine.Base.Query.GeometryHash(BH.oM.Base.IBHoMObject)")]
        [Description("Returns the geometrical identity of any IBHoMObject, useful for diffing. " +
            "\nThe geometrical identity is computed by extracting the geometry of the object via the IGeometry() method." +
            "\nThen, the hash is computed as a serialised array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Input("bhomObj", "Input BHoMObject whose geometry will be queried by IGeometry() and which will be used for computing a Geometry Hash.")]
        [Output("geomHash", "Value representing a unique signature of the input object's geometry.")]
        public static string GeometryHash(this IBHoMObject bhomObj, BaseComparisonConfig comparisonConfig = null)
        {
            IGeometry igeom = bhomObj.IGeometry();

            return GeometryHash(igeom, comparisonConfig);
        }

        /***************************************************/

        [Description("Returns a signature of the input geometry, useful for diffing." +
            "\nThe hash is computed as a serialised array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Output("geomHash", "Value representing a unique signature of the input geometry.")]
        public static string GeometryHash(this IGeometry igeometry, BaseComparisonConfig comparisonConfig = null)
        {
            return GeometryHash(igeometry, comparisonConfig, null);
        }

        /***************************************************/

        [Description("Returns a signature of the input geometry, useful for diffing." +
            "\nThe hash is computed as a serialised array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Input("igeometry", "Geometry you want to compute the hash for.")]
        [Input("comparisonConfig", "Configurations on how the hash is computed, with options for numerical approximation, type exceptions and many others.")]
        [Input("fullName", "Name of the property that holds the target object to calculate the hash for. This name will be used to seek any matching custom configuration to apply against the `comparisonConfig` input.")]
        [Output("geomHash", "Value representing a unique signature of the input geometry.")]
        public static string GeometryHash(this IGeometry igeometry, BaseComparisonConfig comparisonConfig, string fullName)
        {
            if (igeometry == null)
                return null;

            double[] hashArray = IHashArray(igeometry, comparisonConfig, fullName);
            if (hashArray == null)
                return null;

            byte[] byteArray = GetBytes(hashArray);

            if (m_SHA256Algorithm == null)
                m_SHA256Algorithm = SHA256.Create();

            byte[] byteHash = m_SHA256Algorithm.ComputeHash(byteArray);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteHash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static byte[] GetBytes(this double[] values)
        {
            if (values == null)
                return default;

            return values.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
        }

        /***************************************************/
        /****              Private Fields               ****/
        /***************************************************/

        private static HashAlgorithm m_SHA256Algorithm = SHA256.Create();
    }
}