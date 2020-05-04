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

        public static BH.oM.Graphics.RenderMesh RenderMesh(this Point point, RenderMeshOptions renderMeshOptions = null, bool isSubObject = false)
        {
            if (isSubObject) // if it is a property of another object (e.g. a Line) do not display its endpoints as spheres.
                return null;

            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            double radius = 0.12 * renderMeshOptions.Element0DScale;

            // // - Sphere still doesn't work properly
            //Sphere sphere = BH.Engine.Geometry.Create.Sphere(point, radius);

            //return sphere.RenderMesh(renderMeshOptions);

            // // - For now just return a little cube instead of a sphere.
            Cuboid cuboid = BH.Engine.Geometry.Create.Cuboid(BH.Engine.Geometry.Create.CartesianCoordinateSystem(point, BH.Engine.Geometry.Create.Vector(1,0,0), BH.Engine.Geometry.Create.Vector(0, 1, 0)), radius, radius, radius);
            return cuboid.RenderMesh(renderMeshOptions);
        }
   
    } 
}