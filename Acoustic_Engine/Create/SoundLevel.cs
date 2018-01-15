using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SoundLevel SoundLevel(double value, int receiverID, int speakerID, Frequency frequency)
        {
            return new SoundLevel()
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
