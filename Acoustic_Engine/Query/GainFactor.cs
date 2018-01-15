using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double GainFactor(this Speaker speaker, double angle, Frequency frequency)
        {
            double gains = speaker.Gains[frequency];
            switch (frequency)
            {
                case Frequency.Hz63:
                case Frequency.Hz125:
                case Frequency.Hz250:
                case Frequency.Hz500:
                case Frequency.Hz1000:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz2000:
                case Frequency.Hz4000:
                case Frequency.Hz8000:
                case Frequency.Hz16000:
                    return (-18 * angle / 150 - 2);
                default:
                    return 0;
            }
        }

        /***************************************************/
    }
}
