using BH.oM.Architecture.Elements;

namespace BH.Engine.Architecture.Elements
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Level Level(double elevation)
        {
            return new Level
            {
                Elevation = elevation
            };
        }

        /***************************************************/
    }
}
