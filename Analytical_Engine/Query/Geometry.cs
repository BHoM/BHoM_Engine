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


using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.oM.Analytical.Fragments;
using BH.Engine.Base;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a INode as a Point. Method required for automatic display in UI packages.")]
        [Input("node", "INode to get the Point from.")]
        [Output("point", "The geometry of the INode.")]
        public static Point Geometry(this INode node)
        {
            return node.Position;
        }

        /***************************************************/

        [Description("Gets the geometry of a ILink as its centreline. Method required for automatic display in UI packages.")]
        [Input("link", "ILink to get the centreline geometry from.")]
        [Output("line", "The geometry of the ILink as its centreline.")]
        public static Line Geometry<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            return new Line { Start = link.StartNode.Position, End = link.EndNode.Position };
        }

        /***************************************************/

        [Description("Gets the geometry of a IEdge as its Curve. Method required for automatic display in UI packages.")]
        [Input("edge", "IEdge to get the curve geometry from.")]
        [Output("curve", "The geometry of the IEdge as its Curve.")]
        public static ICurve Geometry(this IEdge edge)
        {
            return edge.Curve;
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IPanel at its centre. Method required for automatic display in UI packages.")]
        [Input("panel", "IPanel to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the analytical IPanel at its centre.")]
        public static PlanarSurface Geometry<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return new PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                    panel.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList());
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IOpening as an outline curve. Method required for automatic display in UI packages.")]
        [Input("opening", "IOpening to get the outline geometry from.")]
        [Output("outline", "The geometry of the analytical IOpening.")]
        public static PolyCurve Geometry<TEdge>(this IOpening<TEdge> opening)
            where TEdge : IEdge

        {
            return new PolyCurve { Curves = opening.Edges.Select(x => x.Curve).ToList() };
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical ISurface at its centre. Method required for automatic display in UI packages.")]
        [Input("surface", "Analytical ISurface to get the geometrical Surface geometry from.")]
        [Output("surface", "The underlying surface geometry of the analytical ISurface at its centre.")]
        public static IGeometry Geometry(this BH.oM.Analytical.Elements.ISurface surface)
        {
            return surface.Extents;
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IMesh as a geometrical Mesh. A geometrical mesh only supports 3 and 4 nodes faces, while a FEMesh does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("mesh", "Analytical IMesh to get the mesh geometry from.")]
        [Output("mesh", "The geometry of the IMesh as a geometrical Mesh.")]
        public static Mesh Geometry<TNode, TFace>(this IMesh<TNode, TFace> mesh)
            where TNode : INode
            where TFace : IFace
        {
            Mesh geoMesh = new Mesh();

            geoMesh.Vertices = mesh.Nodes.Select(x => x.Position).ToList();

            geoMesh.Faces.AddRange(mesh.Faces.Geometry());

            return geoMesh;
        }

        /***************************************************/

        [Description("Gets the geometry of a collection of IFaces as a geometrical Mesh's Faces. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("faces", "Analytical IFaces to get the mesh faces geometry from.")]
        [Output("faces", "The geometry of the IFaces as geometrical Mesh Faces.")]
        public static IEnumerable<Face> Geometry<TFace>(this IEnumerable<TFace> faces)
            where TFace : IFace
        {
            List<Face> result = new List<Face>();
            foreach (IFace feFace in faces)
            {
                Face face = Geometry(feFace);
                if (face != null)
                    result.Add(face);
            }
            return result;
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IFace as a geometrical Mesh's Face. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("face", "Analytical IFace to get the mesh face geometry from.")]
        [Output("face", "The geometry of the IFace as geometrical Mesh Face.")]
        public static Face Geometry(this IFace face)
        {

            if (face.NodeListIndices.Count < 3)
            {
                Reflection.Compute.RecordError("Insuffiecient node indices");
                return null;
            }
            if (face.NodeListIndices.Count > 4)
            {
                Reflection.Compute.RecordError("To high number of node indices. Can only handle triangular and quads");
                return null;
            }

            Face geomFace = new Face();

            geomFace.A = face.NodeListIndices[0];
            geomFace.B = face.NodeListIndices[1];
            geomFace.C = face.NodeListIndices[2];

            if (face.NodeListIndices.Count == 4)
                geomFace.D = face.NodeListIndices[3];

            return geomFace;
        }

        /***************************************************/

        [Description("Gets the geometry of a IRegion as its Perimiter curve. Method required for automatic display in UI packages.")]
        [Input("region", "IRegion to get the curve geometry from.")]
        [Output("curve", "The geometry of the IRegion as its Perimiter curve.")]
        public static ICurve Geometry(this IRegion region)
        {
            return region.Perimeter;
        }

        /***************************************************/

        [Description("Gets the geometry of a Graph. Method required for automatic display in UI packages.")]
        [Input("graph", "Graph to get the geometry from.")]
        [Output("Composite Geometry", "The CompositeGeometry geometry of the Graph.")]
        public static CompositeGeometry Geometry(this Graph graph)
        {
            List<IGeometry> geometries = new List<IGeometry>();
            Graph spatialGraph = graph.GraphView(new SpatialView());

            if (spatialGraph.Entities.Count == 0 || spatialGraph.Relations.Count == 0)
                return BH.Engine.Geometry.Create.CompositeGeometry(geometries);

            return SpatialGraphGeometry(spatialGraph);

        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static CompositeGeometry SpatialGraphGeometry(Graph spatialGraph)
        {
            List<IGeometry> geometries = new List<IGeometry>();

            foreach (KeyValuePair<System.Guid, IBHoMObject> kvp in spatialGraph.Entities)
            {
                if (kvp.Value is IElement0D)
                {
                    IElement0D entity = kvp.Value as IElement0D;
                    geometries.Add(entity.IGeometry());
                }
            }
            foreach (IRelation relation in spatialGraph.Relations)
                geometries.Add(relation.RelationArrow());

            return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
        }
        
    }
}
