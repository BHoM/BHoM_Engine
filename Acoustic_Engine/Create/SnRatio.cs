using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SnRatio SnRatio(double value, int receiverID, int speakerID, Frequency frequency)
        {
            return new SnRatio()
            {
                Value = value,
                ReceiverID = receiverID,
                SpeakerID = speakerID,
                Frequency = frequency
            };
        }

        /***************************************************/
    }
}
