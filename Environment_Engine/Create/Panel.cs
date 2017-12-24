using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(ISurface surface, string type = "")
        {
            return new Panel
            {
                Surface = surface,
                Type = type
            };
        }

        /***************************************************/
    }
}
