using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Storey Storey(string name, double elevation, double height)
        {
            return new Storey { Name = name, Elevation = elevation, Height = height };
        }

        /***************************************************/
    }
}
