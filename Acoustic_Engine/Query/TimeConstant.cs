using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double TimeConstant(double revTime)
        {
            return revTime / Constants.SabineConstant;
        }
        
        /***************************************************/

        public static double TimeConstant(this RT60 revTime)
        {
            return revTime.Value / Constants.SabineConstant;
        }

        /***************************************************/
    }
}
