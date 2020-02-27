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
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Common;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a Node as a Point. Method required for automatic display in UI packages.")]
        [Input("node", "Node to get the Point from.")]
        [Output("point", "The geometry of the Node")]
        public static Point Geometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        [Description("Gets the geometry of a Bar as its centreline. Method required for automatic display in UI packages.")]
        [Input("bar", "Bar to get the centreline geometry from.")]
        [Output("line", "The geometry of the Bar as its centreline.")]
        public static Line Geometry(this Bar bar)
        {
            return bar.Centreline();
        }

        /***************************************************/

        [Description("Gets the geometry of a Edge as its Curve. Method required for automatic display in UI packages.")]
        [Input("edge", "Edge to get the curve geometry from.")]
        [Output("curve", "The geometry of the Edge as its Curve.")]
        public static ICurve Geometry(this Edge edge)
        {
            return edge.Curve;
        }

        /***************************************************/

        [Description("Gets the geometry of a structural Surface at its centre. Method required for automatic display in UI packages.")]
        [Input("surface", "Structural Surface to get the geometrical Surface geometry from.")]
        [Output("surface", "The geometry of the structural Sufarce at its centre.")]
        public static IGeometry Geometry(this Surface surface)
        {
            return surface.Extents;
        }

        /***************************************************/

        [Description("Gets the geometry of a structural Panel at its centre. Method required for automatic display in UI packages.")]
        [Input("panel", "Panel to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the structural Panel at its centre.")]
        public static IGeometry Geometry(this Panel panel)
        {
            return Engine.Geometry.Create.PlanarSurface(
                Engine.Geometry.Modify.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                panel.Openings.SelectMany(x => Engine.Geometry.Modify.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList()
            );
        }

        /***************************************************/

        [Description("Gets the geometry of a ConcreteSection as its profile outlines and reinforcement in the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "ConcreteSection to get outline and reinforcement geometry from.")]
        [Output("outlines", "The geometry of the ConcreteSection as its outline and reinforment curves in the global XY.")]
        public static CompositeGeometry Geometry(this ConcreteSection section)
        {
            if (section.SectionProfile.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.SectionProfile.Edges);
            if(section.Reinforcement != null)
                geom.Elements.AddRange(section.Layout().Elements);

            return geom;
        }

        /***************************************************/

        [Description("Gets the geometry of a GeometricalSection as its profile outlines the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "GeometricalSection to get outline geometry from.")]
        [Output("outlines", "The geometry of the GeometricalSection as its outline in the global XY plane.")]
        public static IGeometry Geometry(this IGeometricalSection section)
        {
            return new CompositeGeometry { Elements = section.SectionProfile.Edges.ToList<IGeometry>() };
        }

        /***************************************************/

        [Description("Gets the geometry of a RigidLink as a list of lines between the master node and the slave nodes. Method required for automatic display in UI packages.")]
        [Input("link", "RigidLink to get the line geometry from.")]
        [Output("lines", "The geometry of the RigidLink as a list of master-slave lines.")]
        public static IGeometry Geometry(this RigidLink link)
        {
            List<IGeometry> lines = new List<IGeometry>();

            foreach (Node sn in link.SlaveNodes)
            {
                lines.Add(new Line() { Start = link.MasterNode.Position, End = sn.Position });
            }
            return new CompositeGeometry() { Elements = lines };
        }

        /***************************************************/

        [Description("Gets the geometry of a FEMesh as a geometrical Mesh. A geometrical mesh only supports 3 and 4 nodes faces, while a FEMesh does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("feMesh", "FEMesh to get the mesh geometry from.")]
        [Output("mesh", "The geometry of the FEMesh as a geometrical Mesh.")]
        public static Mesh Geometry(this FEMesh feMesh)
        {
            Mesh mesh = new Mesh();

            mesh.Vertices = feMesh.Nodes.Select(x => x.Position).ToList();

            mesh.Faces.AddRange(feMesh.Faces.Geometry());

            return mesh;
        }

        /***************************************************/

        [Description("Gets the geometry of a collection of FEMeshFaces as a geometrical Mesh's Faces. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("feFaces", "FEMeshFaces to get the mesh faces geometry from.")]
        [Output("faces", "The geometry of the FEMeshFaces as geometrical Mesh Faces.")]
        public static IEnumerable<Face> Geometry(this IEnumerable<FEMeshFace> feFaces)
        {
            List<Face> result = new List<Face>();
            foreach (FEMeshFace feFace in feFaces)
            {
                Face face = Geometry(feFace);
                if (face != null)
                    result.Add(face);
            }
            return result;
        }

        /***************************************************/

        [Description("Gets the geometry of a FEMeshFace as a geometrical Mesh's Face. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("feFace", "FEMeshFace to get the mesh face geometry from.")]
        [Output("face", "The geometry of the FEMeshFace as geometrical Mesh Face.")]
        public static Face Geometry(this FEMeshFace feFace)
        {

            if (feFace.NodeListIndices.Count < 3)
            {
                Reflection.Compute.RecordError("Insuffiecient node indices");
                return null;
            }
            if (feFace.NodeListIndices.Count > 4)
            {
                Reflection.Compute.RecordError("To high number of node indices. Can only handle triangular and quads");
                return null;
            }

            Face face = new Face();

            face.A = feFace.NodeListIndices[0];
            face.B = feFace.NodeListIndices[1];
            face.C = feFace.NodeListIndices[2];

            if (feFace.NodeListIndices.Count == 4)
                face.D = feFace.NodeListIndices[3];

            return face;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the geometry of a SectionProperty, generally as its profile outlines the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "SectionProperty to get outline geometry from.")]
        [Output("outlines", "The geometry of the SectionProperty.")]
        public static IGeometry IGeometry(this ISectionProperty section)
        {
            return Geometry(section as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry Geometry(this object section)
        {
            return null;
        }

        /***************************************************/
        /**** Public Methods - Deprecated               ****/
        /***************************************************/

        [Deprecated("3.1", "Replaced with method for base interface IGeometricalSection.", typeof(Query), "Geometry(this IGeometricalSection section).")]
        public static IGeometry Geometry(this SteelSection section)
        {
            return new CompositeGeometry { Elements = section.SectionProfile.Edges.ToList<IGeometry>() };
        }

        /***************************************************/

        [Deprecated("2.3", "Methods replaced with methods targeting BH.oM.Physical.Elements.IFramingElement.")]
        public static ICurve Geometry(this FramingElement element)
        {
            return element.LocationCurve;
        }

        /***************************************************/
    }

}

