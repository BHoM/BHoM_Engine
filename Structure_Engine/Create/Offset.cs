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
                StartX = startX,
                StartY = startY,
                StartZ = startZ,
                EndX = endX,
                EndY = endY,
                EndZ = endZ
            };
        }

        /***************************************************/
    }
}
