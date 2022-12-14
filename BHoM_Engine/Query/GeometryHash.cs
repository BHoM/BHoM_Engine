/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using System.Collections;
using System.Data;
using BH.oM.Geometry;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the geometrical identity of any IBHoMObject, useful for distance-based comparisons and diffing. " +
            "\nThe geometrical identity is computed by extracting the geometry of the object via the IGeometry() method." +
            "\nThen, the hash is computed as an array representing the coordinate of significant points taken on the geometry." +
            "\nThe number of points is reduced to the minimum essential to determine uniquely any geometry." +
            "\nAdditionally, the resulting points are transformed based on the source geometry type, to remove or minimize collisions." +
            "\n(Any transformation so performed is translational only, in order to support geometrical tolerance, i.e. numerical distance, when comparing GeometryHashes downstream).")]
        [Input("bhomObj", "Input BHoMObject whose geometry will be queried by IGeometry() and which will be used for computing a Geometry Hash.")]
        [Output("geomHash","Array of numbers representing a unique signature of the input object's geometry.")]
        public static double[] GeometryHash(this IBHoMObject bhomObj)
        {
            IGeometry igeom = bhomObj.IGeometry();

            if (igeom == null)
                return null;

            if (m_GeomHashFunc == null)
            {
                MethodInfo mi = Query.ExtensionMethodToCall(igeom, "IGeometryHash");
                m_GeomHashFunc = (Func<IGeometry, double[]>)Delegate.CreateDelegate(typeof(Func<IGeometry, double[]>), mi);
            }

            return m_GeomHashFunc(igeom);
        }

        private static Func<IGeometry, double[]> m_GeomHashFunc = null;
    }
}


