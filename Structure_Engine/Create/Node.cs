using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Base;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node Node(Point position, string name = "")
        {
            return new Node
            {
                Point = new Point(position.X, position.Y, position.Z),
                Name = name
            };
        }
            
    }
}
