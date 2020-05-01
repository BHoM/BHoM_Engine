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
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        public static BH.oM.Graphics.RenderMesh RenderMesh(this Pipe pipe, RenderMeshOptions renderMeshOptions = null)
        {
            renderMeshOptions = renderMeshOptions ?? new RenderMeshOptions();

            double radius = pipe.Radius;

            Line centreLine = pipe.Centreline as Line;
            Polyline centrePolyline = pipe.Centreline as Polyline;

            if (centreLine != null)
                return RenderMesh(centreLine, renderMeshOptions);

            if (centrePolyline != null)
            {
                RenderMesh unifiedMesh = new RenderMesh();

                for (int i = 0; i < centrePolyline.ControlPoints.Count - 2; i++)
                {
                    Line line = BH.Engine.Geometry.Create.Line(centrePolyline.ControlPoints[i], centrePolyline.ControlPoints[i + 1]);
                    RenderMesh m = RenderMesh(line, renderMeshOptions);
                    
                    // Join the meshes
                    m.Faces.ForEach(f => unifiedMesh.Faces.Add(new Face()
                    {
                        A = f.A + unifiedMesh.Faces.Count,
                        B = f.B + unifiedMesh.Faces.Count,
                        C = f.C + unifiedMesh.Faces.Count,
                        D = f.D + unifiedMesh.Faces.Count
                    }));

                    unifiedMesh.Vertices.AddRange(m.Vertices);
                }

                return unifiedMesh;
            }

            BH.Engine.Reflection.Compute.RecordError("RenderMesh for Pipe currently only works with linear pipes (not curves).");
            return null;
        }
    }
}