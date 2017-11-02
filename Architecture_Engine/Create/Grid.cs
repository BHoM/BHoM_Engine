using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Architecture; // TODO: This was BH.oM.Architecture.Element but that was not compiling. Rob, please check this
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Architecture
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static Grid Grid(Point origin, Vector direction)
        {
            Plane plane = new Plane(origin, Geometry.Query.GetCrossProduct(direction, Vector.ZAxis));
            Line line = new Line(origin, origin + direction * 20);
            return new Grid(line);
        }

        /***************************************************/
    }
}
