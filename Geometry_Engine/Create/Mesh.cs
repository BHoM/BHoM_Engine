using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Mesh Mesh(IEnumerable<Point> vertices, IEnumerable<Face> faces)
        {
            return new Mesh
            {
                Vertices = vertices.ToList(),
                Faces = faces.ToList()
            };
        }

        /***************************************************/
    }
}
