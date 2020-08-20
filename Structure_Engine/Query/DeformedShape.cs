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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

using BH.Engine.Geometry;

namespace BH.Engine.Structure
{

    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets deformed shape of a Bar based on BarDisplacements.")]
        [Input("bars", "The Bars to get the deformed shape for. The Bars input here should generally have been pulled from an analysis package to ensure they carry the AdapterNameId.")]
        [Input("barDisplacements", "The displacement results used to compute the deformed shape. The displacements are assumed to be in global coordinates. This list does NOT need to match the Bar input list, grouping is completed by the method.")]
        [Input("adapterNameId", "The CustomData identifier to look for identifying information on for the Bars. This will depend on the software package used, but generally be for example 'Robot_id', 'GSA_id' etc. Try exploding the CustomData of your Bars to find the name of the identifier.")]
        [Input("loadcase", "Loadcase to display results for. Should generally be either an identifier matching the one used in the analysis package that the results were pulled from or a Loadcase/LoadCombination.")]
        [Input("scaleFactor", "Controls by how much the results should be scaled.")]
        [Input("drawSections", "Toggles if output should be just centrelines or include section geometry. Note that currently section geometry only supports displacements, no rotations!")]
        [Output("deformed","The shape of the Bars from the displacements.")]
        public static List<IGeometry> DeformedShape(List<Bar> bars, List<BarDisplacement> barDisplacements, string adapterNameId, object loadcase, double scaleFactor = 1.0, bool drawSections = false)
        {
            barDisplacements = barDisplacements.SelectCase(loadcase);

            List<IGeometry> geom = new List<IGeometry>();

            var resGroups = barDisplacements.GroupBy(x => x.ObjectId.ToString()).ToDictionary(x => x.Key);

            if (drawSections)
                Reflection.Compute.RecordWarning("Display of rotations of sections is not yet supported for deformed shape");

            foreach (Bar bar in bars)
            {
                string id = bar.CustomData[adapterNameId].ToString();

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

        [Description("Gets deformed shape of a FEMesh based on MeshDisplacements.")]
        [Input("meshes", "The FEMeshes to get the deformed shape for. The FEMeshes input here should generally have been pulled from an analysis package to ensure they carry the AdapterNameId.")]
        [Input("meshDisplacements", "The displacement results used to compute the deformed shape.  This input should be a list of MeshResults which in turn should contain results of type MeshDisplacements. The displacements are assumed to be in global coordinates. This list does NOT need to match the FEMesh input list, grouping is completed by the method.")]
        [Input("adapterNameId", "The CustomData identifier to look for identifying information on for the FEMeshes. This will depend on the software package used, but generally be for example 'Robot_id', 'GSA_id' etc. Try exploding the CustomData of your FEMeshes to find the name of the identifier.")]
        [Input("loadcase", "Loadcase to display results for. Should generally be either an identifier matching the one used in the analysis package that the results were pulled from or a Loadcase/LoadCombination.")]
        [Input("scaleFactor", "Controls by how much the results should be scaled.")]
        [Output("deformed", "The shape of the FEMeshes from the displacements.")]
        public static List<Mesh> DeformedShape(List<FEMesh> meshes, List<MeshResult> meshDisplacements, string adapterNameId, object loadcase, double scaleFactor = 1.0)
        {
            meshDisplacements = meshDisplacements.SelectCase(loadcase);

            List<Mesh> defMeshes = new List<Mesh>();

            var resGroups = meshDisplacements.GroupBy(x => x.ObjectId.ToString()).ToDictionary(x => x.Key);

            foreach (FEMesh feMesh in meshes)
            {
                string id = feMesh.CustomData[adapterNameId].ToString();

                List<MeshResult> deformations;

                IGrouping<string, MeshResult> outVal;
                if (resGroups.TryGetValue(id, out outVal))
                    deformations = outVal.ToList();
                else
                    continue;

                MeshResult singleDisp = deformations.Where(x => x.ObjectId.ToString() == id && x.Results.First() is IMeshDisplacement).First();

                defMeshes.Add(DeformedMesh(feMesh, singleDisp.Results.Cast<IMeshDisplacement>(), adapterNameId, scaleFactor));
            }

            return defMeshes;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline DeformedShapeCentreLine(Bar bar, List<IBarDisplacement> deformations, double scaleFactor = 1.0)
        {
            Vector tan = (bar.EndNode.Position - bar.StartNode.Position);
            List<Point> pts = new List<Point>();

            foreach (IBarDisplacement defo in deformations)
            {
                Vector disp = new Vector { X = defo.UX * scaleFactor, Y = defo.UY * scaleFactor, Z = defo.UZ * scaleFactor };
                Point pt = bar.StartNode.Position + tan * defo.Position + disp;
                pts.Add(pt);
            }

            return new Polyline { ControlPoints = pts };
        }


        /***************************************************/


        private static List<Loft> DeformedShapeSection(Bar bar, List<IBarDisplacement> deformations, double scaleFactor = 1.0)
        {
            Vector tan = bar.Tangent();

            List<Point> pts = new List<Point>();

            IEnumerable<ICurve> sectionCurves = bar.Extrude(false).Select(x => (x as BH.oM.Geometry.Extrusion).Curve);

            List<Loft> lofts = new List<Loft>();
            foreach (ICurve sectionCurve in sectionCurves)
            {
                Loft loft = new Loft();
                foreach (IBarDisplacement defo in deformations)
                {
                    //ICurve curve = sectionCurve.IRotate(bar.StartNode.Position, tan, defo.RX * scaleFactor);
                    //Vector disp = unitTan * defo.UX * scaleFactor + yAxis * defo.UY * scaleFactor + normal * defo.UZ * scaleFactor;
                    Vector disp = new Vector { X = defo.UX * scaleFactor, Y = defo.UY * scaleFactor, Z = defo.UZ * scaleFactor };
                    disp += tan * defo.Position;
                    loft.Curves.Add(sectionCurve.ITranslate(disp));
                }
                lofts.Add(loft);
            }


            return lofts;
        }


        /***************************************************/

        private static Mesh DeformedMesh(FEMesh feMesh, IEnumerable<IMeshDisplacement> disps, string adapterNameId, double scaleFactor)
        {
            Mesh mesh = new Mesh();

            foreach (Node node in feMesh.Nodes)
            {
                IMeshDisplacement disp = disps.FirstOrDefault(x => x.NodeId.ToString() == node.CustomData[adapterNameId].ToString());

                if (disp == null)
                {
                    Reflection.Compute.RecordError("Could not find displacement for node with adapter Id: " + node.CustomData[adapterNameId].ToString() + ", from mesh with Id: " + feMesh.CustomData[adapterNameId].ToString());
                    return new Mesh();
                }

                Vector dispVector = disp.Orientation.X * disp.UXX * scaleFactor + disp.Orientation.Y * disp.UYY * scaleFactor + disp.Orientation.Z * disp.UZZ * scaleFactor;

                mesh.Vertices.Add(node.Position + dispVector);

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

