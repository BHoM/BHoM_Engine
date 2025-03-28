/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Data.Collections;
using BH.Engine.Data;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Gets the external edge curves of the Extrusion.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> ExternalEdges(this Extrusion surface)
        {
            ICurve curve = surface.Curve;
            Vector direction = surface.Direction;
            List<ICurve> edges = new List<ICurve>();

            if (!surface.Capped)
            {
                edges.Add(curve);
                ICurve other = curve;
                edges.Add(other.ITranslate(direction));
            }

            if (!curve.IIsClosed())
            {
                Point start = curve.IStartPoint();
                Point end = curve.IEndPoint();
                edges.Add(new Line { Start = start, End = start + direction });
                edges.Add(new Line { Start = end, End = end + direction });
            }

            return edges;
        }

        /***************************************************/

        [Description("Gets the external edge curves of the Loft as the curves of the Loft.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> ExternalEdges(this Loft surface)
        {
            return surface.Curves; //TODO: Is that always correct?
        }

        /***************************************************/

        [Description("Gets the external edge curves of the Pipe. If the Pipe is capped, this returns a circle at each end of the centre curve. If the pipe is uncapped, this method returns an empty list.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> ExternalEdges(this Pipe surface)
        {
            if (!surface.Capped)
            {
                ICurve curve = surface.Centreline;
                return new List<ICurve>()
                {
                    new Circle { Centre = curve.IStartPoint(), Normal = curve.IStartDir(), Radius = surface.Radius },
                    new Circle { Centre = curve.IEndPoint(), Normal = curve.IEndDir(), Radius = surface.Radius }
                };
            }
            else
                return new List<ICurve>();
        }

        /***************************************************/

        [Description("Gets the external edge curves of the PlanarSurface as its ExternalBoundary.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> ExternalEdges(this PlanarSurface surface)
        {
            return new List<ICurve> { surface.ExternalBoundary };
        }

        /***************************************************/

        [Description("Gets the external edge curves of the PolySurface the external edge curves of all of its parts.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> ExternalEdges(this PolySurface surface)
        {
            return surface.Surfaces.SelectMany(x => x.IExternalEdges()).ToList();
        }


        /***************************************************/
        /**** Public Methods - Mesh                     ****/
        /***************************************************/

        [Description("Gets the external edge curves of a mesh as a set of lines. Extraction is done by finding all unique edges in the mesh.")]
        [Input("mesh", "The mesh to extract external edge curves from.")]
        [Input("filterByTopology", "If true, edges with unique topology are returned. If false, edges with unique geometry is returned. Toggle only renders different result for meshes with duplicate nodes.")]
        [Input("tolerance", "Tolerance to be used for identifying duplicate edges. Only used if filterByTopology is false.")]
        [Output("edges", "The external edge curves of the mesh.")]
        public static List<Line> ExternalEdges(this Mesh mesh, bool filterByTopology = false, double tolerance = Tolerance.Distance)
        {
            if (mesh == null)
                return null;

            if (mesh.Faces.Count < 1)
                return new List<Line>();

            if (filterByTopology)
            {
                List<Tuple<int, int>> allEdgeTopologies = new List<Tuple<int, int>>();

                //Creates tuples of indices of edges of each face, ordering the index order so the lowest number is always first (to simplify grouping lower down)
                foreach (Face face in mesh.Faces)
                {
                    allEdgeTopologies.Add(new Tuple<int, int>(Math.Min(face.A, face.B), Math.Max(face.A, face.B)));
                    allEdgeTopologies.Add(new Tuple<int, int>(Math.Min(face.B, face.C), Math.Max(face.B, face.C)));
                    if (face.D == -1)
                    {
                        allEdgeTopologies.Add(new Tuple<int, int>(Math.Min(face.C, face.A), Math.Max(face.C, face.A)));
                    }
                    else
                    {
                        allEdgeTopologies.Add(new Tuple<int, int>(Math.Min(face.C, face.D), Math.Max(face.C, face.D)));
                        allEdgeTopologies.Add(new Tuple<int, int>(Math.Min(face.D, face.A), Math.Max(face.D, face.A)));
                    }
                }

                IEnumerable<Tuple<int, int>> unique = allEdgeTopologies.GroupBy(x => x).Where(x => x.Count() == 1).SelectMany(x => x);  //Get out instances that are uniqe

                return unique.Select(x => new Line { Start = mesh.Vertices[x.Item1], End = mesh.Vertices[x.Item2] }).ToList(); //Generate lines based on the unique tuples
            }
            else
            {
                //Filter by geometry. Use domain tree clustering to find items in range of each other, and return unique instances
                List<Line> edges = mesh.Faces.SelectMany(f => f.Edges(mesh)).ToList();

                double sqTol = tolerance * tolerance;

                //Function turning a line into a singluar box at mid point. The boxes are used as a first pass filtering, why this should be sufficient
                Func<Line, DomainBox> toDomainBox = l => { Point mid = l.PointAtParameter(0.5); return Data.Create.DomainBox(new List<double[]> { new double[] { mid.X, mid.X }, new double[] { mid.Y, mid.Y }, new double[] { mid.Z, mid.Z } }); };
                //Function for evaluating the boxes. Square distance between the boxes used (equal to square distance of midpoints of edges)
                Func<DomainBox, DomainBox, bool> treeFunction = (a, b) => a.SquareDistance(b) < sqTol;
                //Function evaluating the two lines against each other. Called as a second level filtering after the initial check based on midpoint is done.
                Func<Line, Line, bool> itemFunction = (a, b) => (a.Start.SquareDistance(b.Start) < sqTol && a.End.SquareDistance(b.End) < sqTol) || // edge[i] == edge[n]
                                                                (a.Start.SquareDistance(b.End) < sqTol && a.End.SquareDistance(b.Start) < sqTol);
                //Cluster
                List<List<Line>> clusteredItems = Data.Compute.DomainTreeClusters(edges, toDomainBox, treeFunction, itemFunction, 1);
                //Return unique items (clusters that have a single item in them)
                return clusteredItems.Where(x => x.Count == 1).SelectMany(x => x).ToList();

            }
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the external edge curves of the ISurface.")]
        [Input("surface", "Surface to extract external edges from.")]
        [Output("edges", "The external edges of the surface.")]
        public static List<ICurve> IExternalEdges(this ISurface surface)
        {
            return ExternalEdges(surface as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<ICurve> ExternalEdges(this ISurface surface)
        {
            Base.Compute.RecordError($"ExternalEdges is not implemented for ISurface of type: {surface.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



