using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node NewInstance(this Node node)
        {
            return new Node();
        }

        /***************************************************/

        public static Bar NewInstance(this Bar bar)
        {
            return new Bar();
        }

        /***************************************************/

        public static Edge NewInstance(this Edge edge)
        {
            return new Edge();
        }

        /***************************************************/

        public static Opening NewInstance(this Opening opening)
        {
            return new Opening();
        }

        /***************************************************/

        public static PanelPlanar NewInstance(this PanelPlanar panelPlanar)
        {
            return new PanelPlanar();
        }

        /***************************************************/
    }
}
