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

using BH.oM.Base;
using BH.oM.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IGeometry IGeometry3D(this IObject obj)
        {
            if (obj == null)
                return null;

            IGeometry geom = Geometry3D(obj as dynamic);

            CompositeGeometry comp = geom as CompositeGeometry;

            if (comp != null && comp.Elements.Count == 1)
                geom = comp.Elements[0];

            return geom;
        }

        /***************************************************/

        public static IGeometry Geometry3D(this CustomObject obj)
        {
            if(obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the 3D Geometry of a null custom object.");
                return null;
            }

            List<IGeometry> geometries = new List<IGeometry>();

            foreach (object item in obj.CustomData.Values)
            {
                IGeometry geometry = item.Geometry3D();
                if (geometry != null)
                    geometries.Add(geometry);
            }

            if (geometries.Count == 1)
                return geometries[0];

            return new CompositeGeometry { Elements = geometries };
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IGeometry Geometry3D(this object obj)
        {
            if (obj is IGeometry)
                return obj as IGeometry;
            else if (obj is IBHoMObject)
                return ((IBHoMObject)obj).IGeometry3D();
            else if (obj is IEnumerable)
            {
                List<IGeometry> geometries = new List<IGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IGeometry geometry = item.Geometry3D();
                    if (geometry != null)
                        geometries.Add(geometry);
                }
                if (geometries.Count() > 0)
                    return new CompositeGeometry { Elements = geometries };
                else
                    return null;
            }
            else
                return null;   
        }

        /***************************************************/

        private static IGeometry Geometry3D(this IObject obj)
        {
            if (Compute.TryRunExtensionMethod(obj, "Geometry3D", out object result))
                return result as IGeometry;
            else
                return null;
        }

        /***************************************************/
    }
}




