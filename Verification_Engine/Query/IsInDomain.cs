namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static bool IsInDomain(double number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            return (domain.Min == double.MinValue || number >= domain.Min - tolerance)
                && (domain.Max == double.MaxValue || number <= domain.Max + tolerance);
        }

        /***************************************************/

        public static bool IsInDomain(long number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            return (domain.Min == double.MinValue || number >= domain.Min - tolerance)
                && (domain.Max == double.MaxValue || number <= domain.Max + tolerance);
        }

        /***************************************************/
    }
}
