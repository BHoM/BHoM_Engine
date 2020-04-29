/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;
using BH.oM.Base;


namespace BH.Engine.Graphics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        // Main interface method
        public static BH.oM.Graphics.RenderMesh IRenderMesh(this IObject obj, RenderMeshOptions renderMeshOptions = null)
        {
            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            if (obj is BH.oM.Graphics.RenderMesh)
                return obj as BH.oM.Graphics.RenderMesh;

            BH.oM.Geometry.Mesh mesh = obj as BH.oM.Geometry.Mesh;
            if (mesh != null)
                return (RenderMesh)mesh;

            return ToRenderMesh(Geometry(obj) as dynamic, renderMeshOptions);
        }

        // Fallback
        private static BH.oM.Graphics.RenderMesh ToRenderMesh(this IGeometry geom, RenderMeshOptions renderMeshOptions = null)
        {
            BH.Engine.Reflection.Compute.RecordError($"Failed to find a method to compute the Mesh representation of {geom.GetType().Name}");
            return null;
        }

        // Needed to pick geometry for any IObject
        private static IGeometry Geometry(this object obj)
        {
            if (obj is IGeometry)
                return obj as IGeometry;
            else if (obj is IBHoMObject)
                return BH.Engine.Base.Query.IGeometry(((IBHoMObject)obj));
            else if (obj is IEnumerable)
            {
                List<IGeometry> geometries = new List<IGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IGeometry geometry = item.Geometry();
                    if (geometry != null)
                        geometries.Add(geometry);
                }
                if (geometries.Count() > 0)
                    return new CompositeGeometry { Elements = geometries.ToList() };
                else
                    return null;
            }
            else
                return null;
        }
    } 
}