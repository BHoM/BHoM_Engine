﻿using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IBHoMGeometry> Geometry(this ConcreteSection section)
        {
            if (section.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.Edges);
            geom.Elements.AddRange(section.Layout().Elements);

            return geom.Elements;
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this Bar bar)
        {
            return new Line { Start = bar.StartNode.Position, End = bar.EndNode.Position };
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this PanelFreeForm contour)
        {
            return contour.Surface;
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this Storey storey)
        {
            return new Plane { Origin = new Point { X = 0, Y = 0, Z = storey.Elevation }, Normal = new Vector { X = 0, Y = 0, Z = 1 } };
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this IGeometricalSection section)
        {
            return new CompositeGeometry { Elements = section.Edges.ToList<IBHoMGeometry>() };
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this MeshFace meshFace)
        {

            return new Mesh()
            {
                Vertices = meshFace.Nodes.Select(x => x.Position).ToList(),
                Faces = meshFace.Nodes.Count == 3 ? new List<Face>() { new Face() { A = 0, B = 1, C = 2 } } : new List<Face>() { new Face() { A = 0, B = 1, C = 2, D = 3 } }
            };
                
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this RigidLink link)
        {
            List<IBHoMGeometry> lines = new List<IBHoMGeometry>();

            foreach (Node sn in link.SlaveNodes)
            {
                lines.Add(new Line() { Start = link.MasterNode.Position, End = sn.Position });
            }
            return new CompositeGeometry() { Elements = lines };
        }

        /***************************************************/
    }

}
