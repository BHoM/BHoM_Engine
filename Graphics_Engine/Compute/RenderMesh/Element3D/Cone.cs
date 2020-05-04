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
using BHOG = BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;
using BHEG = BH.Engine.Geometry;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        public static BH.oM.Graphics.RenderMesh RenderMesh(this Cone cone, RenderMeshOptions renderMeshOptions = null)
        {
            int coneFaces = 4; // by default this creates a pyramid
            List<double> pointParams = new List<double>();

            if (coneFaces == 4)
                pointParams = new List<double>() { 0.125, 0.375, 0.625, 0.875, 0.125 }; // for 4 corners, make sure the pyramid is oriented like global axes
            else
                pointParams = Enumerable.Range(0, coneFaces + 1).Select(i => (double)((double)i / (double)coneFaces)).ToList();

            pointParams.ForEach(pp => pp += 0.25);

            Circle baseCircle = BH.Engine.Geometry.Create.Circle(cone.Centre, cone.Axis, cone.Radius);
            List<Point> pointsOnBase = pointParams.Select(par => baseCircle.IPointAtParameter(par)).ToList();
            Vector coneHeightVector = BHEG.Modify.Scale(cone.Axis, cone.Height);

            Point topPoint = new Point() {
                X = cone.Centre.X + BHEG.Modify.Project(coneHeightVector, BHOG.Plane.XY).X,
                Y = cone.Centre.Y + BHEG.Modify.Project(coneHeightVector, BHOG.Plane.XY).Y,// Math.Sin(BHEG.Query.Angle(BHEG.Modify.Project(cone.Axis, BHOG.Plane.XY), BHOG.Vector.XAxis)),
                Z = cone.Centre.Z + coneHeightVector.Z
            };

            List<Face> faces = new List<Face>();
            List<Point> vertices = new List<Point>();

            vertices.AddRange(pointsOnBase);
            vertices.Add(topPoint);

            for (int i = 0; i < pointsOnBase.Count - 1; i++)
            {
                faces.Add(new Face() { A = i, B = i + 1, C = vertices.Count - 1});
            }

            return new RenderMesh() { Vertices = vertices.Select(pt => (Vertex)pt).ToList(), Faces = faces };
        }

    }
}