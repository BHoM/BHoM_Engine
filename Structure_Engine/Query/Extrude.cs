/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;

using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes an extrusion of the section along the Bar centreline.")]
        [Input("bar", "The Bar to get an extruded shape from. This requires the property of the Bar to be of a type with a section profile.")]
        [Input("simple", "If false, a full extrusion of the section curves along the Bar will be returned. If true, a geometrical mesh as a bounding box in local coordinates, enclosing the extruded section will be returned.")]
        [Output("extrusion", "The volumetric representation of the Bar as an extrusion or a geometrical bounding box mesh.")]
        public static List<IGeometry> Extrude(this Bar bar, bool simple = false)
        {
            if (bar.IsNull())
                return null;

            if (bar.SectionProperty == null || !(bar.SectionProperty is IGeometricalSection))
                return new List<IGeometry>();

            IProfile sectionProfile = (bar.SectionProperty as IGeometricalSection).SectionProfile;
            Vector tan = bar.Tangent();

            if (sectionProfile is TaperedProfile taperedProfile)
            {
                if (taperedProfile.InterpolationOrder.Any(x => x > 1))
                    Base.Compute.RecordWarning("Extrusion of TaperedProfiles with interpolation order greater than one is not yet supported. Linear interpolation will be used.");

                if (simple)
                    return ExtrudeTaperedSimple(taperedProfile, bar, tan);
                else
                    return ExtrudeTaperedCurves(taperedProfile, bar, tan);
            }
            else
            {
                List<ICurve> secCurves = sectionProfile.Edges.ToList();
                TransformMatrix totalTransform = bar.BarSectionTranformation();

                if (simple)
                    return ExtrudeSimple(secCurves, totalTransform, tan);
                else
                    return ExtrudeFullCurves(secCurves, totalTransform, tan);
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

        private static List<IGeometry> ExtrudeTaperedCurves(TaperedProfile taperedProfile, Bar bar, Vector tangent)
        {
            List<double> positions = taperedProfile.Profiles.Keys.ToList();
            List<IProfile> profiles = taperedProfile.Profiles.Values.ToList();
            List<List<ICurve>> allCurveSets = new List<List<ICurve>>();

            for (int i = 0; i < positions.Count; i++)
            {
                List<ICurve> secCurves = profiles[i].Edges.ToList();
                TransformMatrix transform = bar.BarSectionTranformation(positions[i]);

                List<PolyCurve> joined = Engine.Geometry.Compute.IJoin(secCurves);
                List<ICurve> transformed = new List<ICurve>();
                for (int k = 0; k < joined.Count; k++)
                {
                    ICurve curve = joined[k];
                    curve = BH.Engine.Geometry.Modify.ITransform(curve, transform);
                    transformed.Add(curve);
                }

                allCurveSets.Add(transformed);
            }

            List<IGeometry> result = new List<IGeometry>();
            if (allCurveSets.Count > 1)
            {
                int curveCount = allCurveSets[0].Count;
                for (int c = 0; c < curveCount; c++)
                {
                    for (int p = 0; p < allCurveSets.Count - 1; p++)
                    {
                        if (c < allCurveSets[p].Count && c < allCurveSets[p + 1].Count)
                            result.Add(Engine.Geometry.Create.Loft(new List<ICurve> { allCurveSets[p][c], allCurveSets[p + 1][c] }));
                    }
                }
            }

            return result;
        }

        /***************************************************/

        private static List<IGeometry> ExtrudeTaperedSimple(TaperedProfile taperedProfile, Bar bar, Vector tangent)
        {
            List<IGeometry> result = new List<IGeometry>();
            List<double> positions = taperedProfile.Profiles.Keys.ToList();
            List<IProfile> profiles = taperedProfile.Profiles.Values.ToList();

            for (int i = 0; i < positions.Count - 1; i++)
            {
                List<ICurve> secCurves = profiles[i].Edges.ToList();
                TransformMatrix transform = bar.BarSectionTranformation(positions[i]);
                Vector subTangent = tangent * (positions[i + 1] - positions[i]);
                result.AddRange(ExtrudeSimple(secCurves, transform, subTangent));
            }

            return result;
        }

        /***************************************************/
    }
}







