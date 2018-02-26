using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Rasti Rasti(double value, int receiverID)
        {
            return new Rasti()
            {
                Value = value,
                ReceiverID = receiverID,
            };
        }

        /***************************************************/
    }
}
