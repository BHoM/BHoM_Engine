using BH.oM.Acoustic;
using BH.oM.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Receiver Receiver(Point location)
        {
            return new Receiver()
            {
                Location = location,
            };
        }

        /***************************************************/

        public static Receiver Receiver(Point location, string category)
        {
            return new Receiver()
            {
                Location = location,
                Category = category,
            };
        }

        /***************************************************/
    }
}
