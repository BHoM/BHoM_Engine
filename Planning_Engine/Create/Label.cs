using BH.oM.Planning;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Label Label(string name, string colour)
        {
            return new Label
            {
                Name = name,
                Colour = colour,
            };
        }

        /***************************************************/
    }
}
