/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
using BH.oM.Geometry.ShapeProfiles;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IGeometry> Extrude(this Bar bar, bool simple = false)
        {

            System.Reflection.PropertyInfo prop = bar.SectionProperty.GetType().GetProperty("SectionProfile");

            IProfile profile;

            if (prop != null)
                profile = prop.GetValue(bar.SectionProperty) as IProfile;
            else
                return null;// bar.Geometry();

            List<ICurve> secCurves = profile.Edges.ToList();

            Vector trans = bar.StartNode.Position - Point.Origin;
            Vector tan = bar.Tangent();


            Vector gX = Vector.XAxis;
            Vector gY = Vector.YAxis;
            Vector gZ = Vector.ZAxis;

            Vector lX = tan.Normalise();
            Vector lZ = bar.Normal();
            Vector lY = lZ.CrossProduct(lX);

            TransformMatrix localToGlobal = new TransformMatrix();

            localToGlobal.Matrix[0, 0] = gX.DotProduct(lX);
            localToGlobal.Matrix[0, 1] = gX.DotProduct(lY);
            localToGlobal.Matrix[0, 2] = gX.DotProduct(lZ);

            localToGlobal.Matrix[1, 0] = gY.DotProduct(lX);
            localToGlobal.Matrix[1, 1] = gY.DotProduct(lY);
            localToGlobal.Matrix[1, 2] = gY.DotProduct(lZ);

            localToGlobal.Matrix[2, 0] = gZ.DotProduct(lX);
            localToGlobal.Matrix[2, 1] = gZ.DotProduct(lY);
            localToGlobal.Matrix[2, 2] = gZ.DotProduct(lZ);
            localToGlobal.Matrix[3, 3] = 1;

            TransformMatrix totalTransform = Engine.Geometry.Create.TranslationMatrix(trans) * localToGlobal * GlobalToSectionAxes;

            if (simple)
                return ExtrudeSimple(secCurves, totalTransform, tan);
            else
                return ExtrudeFullCurves(secCurves, totalTransform, tan);

        }

        /***************************************************/
        /**** Private Property                          ****/
        /***************************************************/


       private static TransformMatrix GlobalToSectionAxes
        {
            get
            {
                Vector gX = Vector.XAxis;
                Vector gY = Vector.YAxis;
                Vector gZ = Vector.ZAxis;

                //Global system vectors, Sections are drawn in global XY plane with y relating to the normal
                Vector lX = Vector.ZAxis;
                Vector lY = Vector.XAxis;
                Vector lZ = Vector.YAxis;

                TransformMatrix transform = new TransformMatrix();



                transform.Matrix[0, 0] = lX.DotProduct(gX);
                transform.Matrix[0, 1] = lX.DotProduct(gY);
                transform.Matrix[0, 2] = lX.DotProduct(gZ);

                transform.Matrix[1, 0] = lY.DotProduct(gX);
                transform.Matrix[1, 1] = lY.DotProduct(gY);
                transform.Matrix[1, 2] = lY.DotProduct(gZ);

                transform.Matrix[2, 0] = lZ.DotProduct(gX);
                transform.Matrix[2, 1] = lZ.DotProduct(gY);
                transform.Matrix[2, 2] = lZ.DotProduct(gZ);

                transform.Matrix[3, 3] = 1;

                return transform;
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IGeometry> ExtrudeFullCurves(List<ICurve> sectionCurves, TransformMatrix matrix, Vector tangent)
        {
            List<IGeometry> extrusions = new List<IGeometry>();

            List<PolyCurve> joined = BH.Engine.Geometry.Compute.IJoin(sectionCurves);

            for (int i = 0; i < joined.Count; i++)
            {
                ICurve curve = joined[i];
                curve = BH.Engine.Geometry.Modify.ITransform(curve, matrix);
                extrusions.Add(new Extrusion() { Curve = curve, Direction = tangent });
            }

            return extrusions;
        }

        /***************************************************/

        private static List<IGeometry> ExtrudeSimple(List<ICurve> sectionCurves, TransformMatrix matrix, Vector tangent)
        {
            BoundingBox box = sectionCurves.First().IBounds();

            for (int i = 1; i < sectionCurves.Count; i++)
            {
                box += sectionCurves[i].IBounds();
            }

            List<Point> pts = new List<Point>();

            pts.Add(new Point { X = box.Min.X, Y = box.Min.Y });
            pts.Add(new Point { X = box.Min.X, Y = box.Max.Y });
            pts.Add(new Point { X = box.Max.X, Y = box.Max.Y });
            pts.Add(new Point { X = box.Max.X, Y = box.Min.Y });

            for (int i = 0; i < pts.Count; i++)
            {
                pts[i] = pts[i].Transform(matrix);
            }

            for (int i = 0; i < 4; i++)
            {
                pts.Add(pts[i] + tangent);
            }

            Mesh mesh = new Mesh() { Vertices = pts };

            mesh.Faces.Add(new Face { A = 0, B = 1, C = 2, D = 3 });
            mesh.Faces.Add(new Face { A = 0, B = 1, C = 5, D = 4 });
            mesh.Faces.Add(new Face { A = 1, B = 2, C = 6, D = 5 });
            mesh.Faces.Add(new Face { A = 2, B = 3, C = 7, D = 6 });
            mesh.Faces.Add(new Face { A = 3, B = 0, C = 4, D = 7 });
            mesh.Faces.Add(new Face { A = 4, B = 5, C = 6, D = 7 });

            return new List<IGeometry> { mesh };

        }

        /***************************************************/
    }
}
