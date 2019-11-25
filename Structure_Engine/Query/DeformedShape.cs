/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;

using BH.Engine.Geometry;

namespace BH.Engine.Structure
{

    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IGeometry> DeformedShape(List<Bar> bars, List<BarDisplacement> barDisplacements, string adapterId, object loadCase, double scaleFactor = 1.0, bool drawSections = false)
        {
            barDisplacements = barDisplacements.SelectCase(loadCase);

            List<IGeometry> geom = new List<IGeometry>();

            var resGroups = barDisplacements.GroupBy(x => x.ObjectId.ToString()).ToDictionary(x => x.Key);

            foreach (Bar bar in bars)
            {
                string id = bar.CustomData[adapterId].ToString();

                List<BarDisplacement> deformations;

                IGrouping<string, BarDisplacement> outVal;
                if (resGroups.TryGetValue(id, out outVal))
                    deformations = outVal.ToList();
                else
                    continue;

                deformations.Sort();
                if (drawSections)
                    geom.AddRange(DeformedShapeSection(bar, deformations, scaleFactor));
                else
                    geom.Add(DeformedShapeCentreLine(bar, deformations, scaleFactor));

            }

            return geom;
        }

        /***************************************************/


        public static List<Mesh> DeformedShape(List<FEMesh> meshes, List<MeshResult> meshDeformations, string adapterId, object loadCase, double scaleFactor = 1.0)
        {
            meshDeformations = meshDeformations.SelectCase(loadCase);

            List<Mesh> defMeshes = new List<Mesh>();

            foreach (FEMesh feMesh in meshes)
            {
                string id = feMesh.CustomData[adapterId].ToString();
                MeshResult deformations = meshDeformations.Where(x => x.ObjectId.ToString() == id && x.Results.First() is MeshDisplacement).First();

                defMeshes.Add(DeformedMesh(feMesh, deformations.Results.Cast<MeshDisplacement>(), adapterId, scaleFactor));
            }

            return defMeshes;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline DeformedShapeCentreLine(Bar bar, List<BarDisplacement> deformations, double scaleFactor = 1.0)
        {
            Vector tan = (bar.EndNode.Position() - bar.StartNode.Position());
            //Vector unitTan = tan.Normalise();
            //Vector normal = bar.Normal();
            //Vector yAxis = normal.CrossProduct(unitTan);

            List<Point> pts = new List<Point>();

            foreach (BarDisplacement defo in deformations)
            {
                Vector disp = new Vector { X = defo.UX * scaleFactor, Y = defo.UY * scaleFactor, Z = defo.UZ * scaleFactor };
                //Vector disp = unitTan * defo.UX * scaleFactor + yAxis * defo.UY * scaleFactor + normal * defo.UZ * scaleFactor;
                Point pt = bar.StartNode.Position() + tan * defo.Position + disp;
                pts.Add(pt);
            }

            return new Polyline { ControlPoints = pts };
        }


        /***************************************************/


        private static List<Loft> DeformedShapeSection(Bar bar, List<BarDisplacement> deformations, double scaleFactor = 1.0)
        {
            Vector tan = bar.Tangent();
            Vector unitTan = tan.Normalise();
            Vector normal = bar.Normal();
            Vector yAxis = normal.CrossProduct(unitTan);


            List<Point> pts = new List<Point>();

            IEnumerable<ICurve> sectionCurves = bar.Extrude(false).Select(x => (x as BH.oM.Geometry.Extrusion).Curve);

            List<Loft> lofts = new List<Loft>();
            foreach (ICurve sectionCurve in sectionCurves)
            {
                Loft loft = new Loft();
                foreach (BarDisplacement defo in deformations)
                {
                    ICurve curve = sectionCurve.IRotate(bar.StartNode.Position(), unitTan, defo.RX * scaleFactor);
                    //Vector disp = unitTan * defo.UX * scaleFactor + yAxis * defo.UY * scaleFactor + normal * defo.UZ * scaleFactor;
                    Vector disp = new Vector { X = defo.UX * scaleFactor, Y = defo.UY * scaleFactor, Z = defo.UZ * scaleFactor };
                    disp += tan * defo.Position;
                    loft.Curves.Add(curve.ITranslate(disp));
                }
                lofts.Add(loft);
            }


            return lofts;
        }


        /***************************************************/

        private static Mesh DeformedMesh(FEMesh feMesh, IEnumerable<MeshDisplacement> disps, string adapterId, double scaleFactor)
        {
            Mesh mesh = new Mesh();

            foreach (Node node in feMesh.Nodes)
            {
                MeshDisplacement disp = disps.FirstOrDefault(x => x.NodeId.ToString() == node.CustomData[adapterId].ToString());

                if (disp == null)
                {
                    Reflection.Compute.RecordError("Could not find displacement for node with adapter Id: " + node.CustomData[adapterId].ToString() + ", from mesh with Id: " + feMesh.CustomData[adapterId].ToString());
                    return new Mesh();
                }

                Vector dispVector = disp.Orientation.X * disp.UXX * scaleFactor + disp.Orientation.Y * disp.UYY * scaleFactor + disp.Orientation.Z * disp.UZZ * scaleFactor;

                mesh.Vertices.Add(node.Position() + dispVector);

            }

            foreach (FEMeshFace feFace in feMesh.Faces)
            {
                if (feFace.NodeListIndices.Count < 3)
                {
                    Reflection.Compute.RecordError("Insuffiecient node indices");
                    continue;
                }
                if (feFace.NodeListIndices.Count > 4)
                {
                    Reflection.Compute.RecordError("To high number of node indices. Can only handle triangular and quads");
                    continue;
                }

                Face face = new Face();

                face.A = feFace.NodeListIndices[0];
                face.B = feFace.NodeListIndices[1];
                face.C = feFace.NodeListIndices[2];

                if (feFace.NodeListIndices.Count == 4)
                    face.D = feFace.NodeListIndices[3];

                mesh.Faces.Add(face);
            }

            return mesh;

        }


        /***************************************************/
    }
}
