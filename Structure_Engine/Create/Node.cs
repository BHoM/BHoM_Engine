using BH.oM.Structural.Elements;
using BH.oM.Geometry;

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
