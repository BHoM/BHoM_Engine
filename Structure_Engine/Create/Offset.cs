using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Offset Offset(double startX, double startY, double startZ, double endX, double endY, double endZ)
        {
            return new Offset
            {
                Start = new oM.Geometry.Vector() { X = startX, Y = startY, Z = startZ },
                End = new oM.Geometry.Vector() { X = endX, Y = endY, Z = endZ }
            };
        }

        /***************************************************/
    }
}
